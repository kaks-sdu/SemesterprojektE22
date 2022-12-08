import os
import tempfile
import json
import flask
from flask import request, abort
from flask_cors import CORS
from sentence_transformers import SentenceTransformer, util
import whisper
from pyhive import hive

app = flask.Flask(__name__)
CORS(app)
app.config['CORS_HEADERS'] = 'Content-Type'

conn = None

def load_settings():
    '''
    Load settings from settings.txt

    Returns:
        dict: settings
            Contains the following keys:
                - 'model_size': model size
                - 'language': language to detect
    '''
    print('Loading settings')
    # Load the settings from settings.json
    with open('settings.json') as f:
        settings = json.load(f)

    print('Successfully loaded settings')
    return settings

def setup_db(settings):
    print('Setting up database connection')
    #establishing the connection
    print(settings)
    conn = hive.Connection(settings['host'], port=settings['port'], username=settings['user'])
    print('Successfully setup database connection')

def load_models(settings):
    '''
    Load the whisper model based on the settings

    Returns:
        whisper_model: the whisper model
        semantic_model: the semantic model
    '''
    print('Loading models')
    model_size = settings['model_size']
    language = settings['language']
    if language == 'english' and model_size != 'large':
            model_size = model_size + '.en'

    whisper_model = whisper.load_model(model_size)

    semantic_model = SentenceTransformer(settings['semantic_model'])
    print('Successfully loaded models')
    return whisper_model, semantic_model


def transcribe_file(save_path):
    if settings['language'] == 'english':
        result = whisper_model.transcribe(save_path, language='english')
    else:
        result = whisper_model.transcribe(save_path)

    transcribed_text = result['text']

    transcribed_text_embedding = semantic_model.encode(transcribed_text)
    questions = settings['questions']
    
    # Loop through the questions and find the most similar one

    result = {
        'transcribed_text': transcribed_text,
        'best_score': 0,
        'best_question': '',
        'index': -1
    }
    
    for i, question in enumerate(questions):
        question_embedding = semantic_model.encode(question)
        score = util.cos_sim(transcribed_text_embedding, question_embedding)
        if score > result['best_score']:
            result['best_score'] = score
            result['best_question'] = question
            result['index'] = i

    if (result['index']) == -1:     
        raise Exception("No questions found")

    return result

def evalute_question(question):
    querry = None
    if (question == "which repositories receive the most updates"):
        querry = "SELECT repo.name, COUNT(payload.commits) AS count FROM push_events GROUP BY repo.name ORDER BY count DESC LIMIT 10; "
    elif (question == "what are the top messages"):
        querry = "SELECT payload.commits.message, COUNT(*) AS count FROM push_events GROUP BY payload.commits.message ORDER BY count DESC LIMIT 25;"
    elif (question == "who are the top authors"):
        querry = "SELECT payload.commits.author.name, COUNT(*) AS count FROM push_events GROUP BY payload.commits.author.name ORDER BY count DESC LIMIT 25;"
    elif (question == "how long are push requests messages on average"):
        raise Exception("Could not generate querry")

    if (querry == None):
        raise Exception("Unknown question")

    #if True:
    #    return querry

    cursor = conn.cursor()
    cursor.execute(querry)
    print(cursor.fetchone())
    # print(cursor.fetchall())
    return cursor.fetchone()
    

@app.route('/transcribe', methods=['POST'])
def transcribe():
    if request.method != 'POST':
        return abort(400)

    temp_dir = tempfile.mkdtemp()
    save_path = os.path.join(temp_dir, 'audio.wav')

    audio_file = request.files['audio_data']
    audio_file.save(save_path)

    question = transcribe_file(save_path)
    answer = evalute_question(question['best_question'])

    json_response = {
        "question": question['best_question'],
        "answer": answer,
        'data': {
            "transcribed_text": question['transcribed_text'].strip(),
            "index": question['index'],
            "score": question['best_score'].item()
        }
    }

    return json.dumps(json_response)


if __name__ == '__main__':
    print('Starting service')
    settings = load_settings()
    setup_db(settings['database'])
    whisper_model, semantic_model = load_models(settings)
    app.run(host='0.0.0.0', port=3000)