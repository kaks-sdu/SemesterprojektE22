import os
import tempfile
import json
import flask
from flask import request, abort
from flask_cors import CORS
from sentence_transformers import SentenceTransformer, util
import whisper

app = flask.Flask(__name__)
CORS(app)
app.config['CORS_HEADERS'] = 'Content-Type'

def load_settings():
    '''
    Load settings from settings.txt

    Returns:
        dict: settings
            Contains the following keys:
                - 'model_size': model size
                - 'language': language to detect
    '''
    # Load the settings from settings.json
    with open('settings.json') as f:
        settings = json.load(f)
    return settings

def load_models(settings):
    '''
    Load the whisper model based on the settings

    Returns:
        whisper_model: the whisper model
        semantic_model: the semantic model
    '''
    model_size = settings['model_size']
    language = settings['language']
    if language == 'english' and model_size != 'large':
            model_size = model_size + '.en'

    whisper_model = whisper.load_model(model_size)

    semantic_model = SentenceTransformer(settings['semantic_model'])
    return whisper_model, semantic_model


@app.route('/transcribe', methods=['POST'])
def transcribe():
    if request.method == 'POST':
        temp_dir = tempfile.mkdtemp()
        save_path = os.path.join(temp_dir, 'audio.wav')

        audio_file = request.files['audio_data']
        audio_file.save(save_path)

        if settings['language'] == 'english':
            result = whisper_model.transcribe(save_path, language='english')
        else:
            result = whisper_model.transcribe(save_path)

        transcribed_text = result['text']

        transcribed_text_embedding = semantic_model.encode(transcribed_text)
        questions = settings['questions']
        
        # Loop through the questions and find the most similar one
        best_score = 0
        best_question = ''
        for i, question in enumerate(questions):
            question_embedding = semantic_model.encode(question)
            score = util.cos_sim(transcribed_text_embedding, question_embedding)
            if score > best_score:
                best_score = score
                best_question = question
                index = i

        json_response = {
            "transcribed_text": transcribed_text.strip(),
            "best_question": best_question,
            "best_question_index": index,
            "best_score": best_score.item()
        }

        return json.dumps(json_response)
    else:
        return abort(400)


if __name__ == '__main__':
    settings = load_settings()
    whisper_model, semantic_model = load_models(settings)
    app.run(host='0.0.0.0', port=3000)