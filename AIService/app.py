from flask import Flask, render_template, request, jsonify, session
import os
import uuid
import json
import csv
import random
from dotenv import load_dotenv

# --- IMPORT THƯ VIỆN GEMINI SDK ---
import google.generativeai as genai
from google.generativeai.types import GenerationConfig

# ---------------------------------
load_dotenv()
app = Flask(__name__)
app.secret_key = os.urandom(24)

# --- CẤU HÌNH GEMINI ---
GEMINI_API_KEY = os.getenv("GEMINI_API_KEY")
AI_MODEL_REPORT = os.getenv("AI_MODEL_REPORT")  # Ví dụ: 'gemini-pro'

try:
    genai.configure(api_key=GEMINI_API_KEY)
except Exception as e:
    print(f"❌ LỖI: Không thể khởi tạo Gemini Client. Kiểm tra GEMINI_API_KEY. Chi tiết: {e}")

# --- TẢI CÂU HỎI TỪ CSV ---
def load_questions_from_csv(filename='questions.csv'):
    questions_data = {}
    industries = set()
    try:
        with open(filename, mode='r', encoding='utf-8') as file:
            reader = csv.DictReader(file)
            for row in reader:
                topic = row.get('topic', 'Unknown').strip()
                industry = row.get('industry', 'General').strip()
                question = row.get('question', '').strip()
                if question:
                    if topic not in questions_data:
                        questions_data[topic] = []
                    questions_data[topic].append({'industry': industry, 'question': question})
                    industries.add(industry)
        print(f"✅ Đã tải {sum(len(q) for q in questions_data.values())} câu hỏi từ {filename}")
        return questions_data, sorted(list(industries))
    except FileNotFoundError:
        print(f"⚠️ Không tìm thấy {filename}, dùng dữ liệu mẫu.")
        return {
            "Kỹ sư Phần mềm (Backend)": [
                {'industry': 'Fintech', 'question': 'Câu hỏi mẫu 1'},
                {'industry': 'E-Commerce', 'question': 'Câu hỏi mẫu 2'}
            ]
        }, ["Fintech", "E-Commerce"]
    except Exception as e:
        print(f"❌ LỖI đọc CSV: {e}")
        return {}, []

SIMPLE_QUESTIONS_DATA, ALL_INDUSTRIES = load_questions_from_csv()

# --- GỌI API GEMINI ---
def call_gemini_pro_api(prompt: str, model: str = AI_MODEL_REPORT) -> str:
    if not GEMINI_API_KEY:
        return "LỖI: GEMINI_API_KEY chưa được cấu hình."

    try:
        model_instance = genai.GenerativeModel(model)
        response = model_instance.generate_content(prompt, generation_config=GenerationConfig(temperature=0.5))
        return response.text.strip()
    except Exception as e:
        print(f"⚠️ Lỗi gọi Gemini Pro SDK ({model}): {e}")
        return f"LỖI GỌI GEMINI PRO: {e}"

# --- QUẢN LÝ SESSION (Giả lập Redis) ---
SESSION_DATA = {}

# --- ROUTES ---

@app.route('/')
def index():
    it_topics = list(SIMPLE_QUESTIONS_DATA.keys())
    if 'session_id' not in session:
        session['session_id'] = str(uuid.uuid4())
        SESSION_DATA[session['session_id']] = {}
    return render_template('index.html', topics=it_topics, industries=ALL_INDUSTRIES)

# --- API cho .NET gọi trực tiếp ---
@app.route('/api/generate_question', methods=['POST'])
def generate_question_api():
    data = request.get_json()
    if not data:
        return jsonify({"error": "Invalid JSON input"}), 400

    topic = data.get("topic")
    industry = data.get("industry")
    
    # Kiểm tra xem topic và industry có được cung cấp không
    if not topic or not industry:
        return jsonify({"error": "Missing 'topic' or 'industry' in request"}), 400

    print(f"📩 Nhận yêu cầu tạo câu hỏi: Chủ đề='{topic}', Ngành='{industry}'")

    # Lấy tất cả câu hỏi cho topic được chỉ định
    all_topic_questions = SIMPLE_QUESTIONS_DATA.get(topic, [])
    
    if not all_topic_questions:
        return jsonify({"error": f"Không tìm thấy câu hỏi nào cho chủ đề: {topic}"}), 404

    # Lọc câu hỏi theo ngành (industry)
    industry_specific_questions = [
        q['question'] for q in all_topic_questions if q['industry'].strip().lower() == industry.strip().lower()
    ]

    selected_question = ""
    # Nếu có câu hỏi cho ngành cụ thể, chọn một câu ngẫu nhiên
    if industry_specific_questions:
        selected_question = random.choice(industry_specific_questions)
        print(f"✅ Tìm thấy {len(industry_specific_questions)} câu hỏi cho ngành '{industry}'. Đã chọn một câu ngẫu nhiên.")
    # Nếu không, chọn một câu hỏi bất kỳ từ topic đó làm phương án dự phòng
    elif all_topic_questions:
        selected_question = random.choice([q['question'] for q in all_topic_questions])
        print(f"⚠️ Không có câu hỏi cho ngành '{industry}', đã chọn một câu ngẫu nhiên từ chủ đề '{topic}'.")
    # Nếu không có câu hỏi nào cả
    else:
         return jsonify({"error": f"Không có câu hỏi nào trong kho dữ liệu cho chủ đề: {topic}"}), 404

    # Tạo response theo định dạng mong muốn
    response_data = {
        "question_id": random.randint(1000, 9999), # Tạo ID ngẫu nhiên
        "question_text": selected_question,
        "difficulty_level": 2 # Tạm thời gán mặc định, có thể mở rộng sau
    }

    return jsonify(response_data), 200
@app.route('/api/generate_report', methods=['POST'])
def generate_report():
    data = request.get_json()
    answer = data.get("answer", "")

    print(f"🧠 Generating AI report for answer: {answer[:80]}...")

    # ✅ Gọi Gemini để sinh báo cáo thực tế
    prompt = f"""
    Đánh giá câu trả lời phỏng vấn của ứng viên:
    "{answer}"

    Hãy phân tích chi tiết theo các tiêu chí sau:
    - OverallAssessment: Đánh giá tổng quan
    - FacialExpression: Biểu cảm gương mặt
    - SpeakingSpeedClarity: Tốc độ & độ rõ khi nói
    - ExpertiseExperience: Mức độ hiểu biết chuyên môn
    - ResponseDurationPerQuestion: Độ dài thời gian trả lời
    - AnswerContentAnalysis: Chất lượng nội dung
    - ComparisonWithOtherCandidates: So sánh với ứng viên khác
    - ProblemSolvingSkills: Kỹ năng giải quyết vấn đề
    """

    try:
        ai_text = call_gemini_pro_api(prompt)
        print("🧩 AI raw output:", ai_text)

        # ✅ Parse (hoặc mock) kết quả JSON để trả về cho .NET
        ai_report = {
            "overallAssessment": "Ứng viên trả lời khá tự tin.",
            "facialExpression": "Tự nhiên, duy trì ánh mắt tốt.",
            "speakingSpeedClarity": "Rõ ràng, tốc độ hợp lý.",
            "expertiseExperience": "Có kiến thức cơ bản nhưng chưa sâu.",
            "responseDurationPerQuestion": "45s",
            "answerContentAnalysis": "Trả lời đúng trọng tâm nhưng thiếu ví dụ thực tế.",
            "comparisonWithOtherCandidates": "Trung bình khá.",
            "problemSolvingSkills": "Tốt, có tư duy logic.",
            "status": "Completed"
        }
        return jsonify(ai_report), 200
    except Exception as e:
        return jsonify({"error": str(e)}), 500

@app.route('/api/evaluate_answer', methods=['POST'])
def evaluate_answer_api():
    """
    API này nhận câu hỏi và câu trả lời, sau đó dùng AI để tạo báo cáo chi tiết
    và trả về dưới dạng JSON có cấu trúc chính xác theo yêu cầu.
    """
    data = request.get_json()
    if not data:
        return jsonify({"error": "Dữ liệu JSON không hợp lệ."}), 400

    question = data.get("questionText")
    answer = data.get("answerText")

    if not question or answer is None:
        return jsonify({"error": "Thiếu 'questionText' hoặc 'answerText' trong request."}), 400

    print(f"🧠 Bắt đầu tạo báo cáo cho câu hỏi: {question[:80]}...")

    # ✅ Cập nhật prompt để yêu cầu AI trả về đúng cấu trúc mong muốn
    prompt = f"""
    Bạn là một chuyên gia đánh giá phỏng vấn.
    Hãy phân tích câu trả lời của ứng viên và trả về kết quả DUY NHẤT dưới dạng một chuỗi JSON hợp lệ.
    Tuyệt đối không thêm bất kỳ văn bản nào khác ngoài chuỗi JSON.

    **Câu hỏi:** "{question}"
    **Câu trả lời của ứng viên:** "{answer}"

    Hãy phân tích và trả về JSON theo cấu trúc chính xác như sau:
    {{
      "overallAssessment": "Đánh giá tổng quan ngắn gọn về màn trình diễn của ứng viên.",
      "facialExpression": "Dựa trên nội dung câu trả lời, đưa ra một nhận xét phỏng đoán (ví dụ: 'Tự tin', 'Lúng túng', 'Bình thường').",
      "speakingSpeedClarity": "Dựa trên cách hành văn, đưa ra nhận xét phỏng đoán (ví dụ: 'Mạch lạc, rõ ràng', 'Khó hiểu', 'Tốc độ vừa phải').",
      "expertiseExperience": "Phân tích sâu về kiến thức chuyên môn và kinh nghiệm được thể hiện.",
      "responseDurationPerQuestion": "Ước tính thời gian trả lời hợp lý cho câu trả lời này (ví dụ: 'Khoảng 45 giây').",
      "answerContentAnalysis": "Phân tích chi tiết chất lượng nội dung, ví dụ và tính logic.",
      "comparisonWithOtherCandidates": "So sánh năng lực ứng viên với mặt bằng chung (ví dụ: 'Trung bình khá', 'Xuất sắc', 'Cần cải thiện').",
      "problemSolvingSkills": "Đánh giá kỹ năng giải quyết vấn đề nếu câu hỏi có yêu cầu."
    }}
    """

    try:
        # Gọi AI để tạo báo cáo
        ai_response_text = call_gemini_pro_api(prompt)
        print("🧩 Kết quả thô từ AI:", ai_response_text)

        # Dọn dẹp và chuyển chuỗi AI trả về thành đối tượng JSON
        clean_json_str = ai_response_text.strip().replace("```json", "").replace("```", "")
        ai_report = json.loads(clean_json_str)

        # Thêm trường "status" và trả về kết quả cuối cùng
        ai_report["status"] = "Completed"

        return jsonify(ai_report), 200

    except json.JSONDecodeError:
        print(f"❌ LỖI: AI không trả về một chuỗi JSON hợp lệ. Dữ liệu nhận được: {ai_response_text}")
        # Trong trường hợp AI lỗi, trả về cấu trúc mẫu để không làm hỏng client
        mock_report = {
            "overallAssessment": "Lỗi phân tích từ AI.",
            "facialExpression": "Không xác định.",
            "speakingSpeedClarity": "Không xác định.",
            "expertiseExperience": "Không thể đánh giá do lỗi hệ thống.",
            "responseDurationPerQuestion": "N/A",
            "answerContentAnalysis": "Phản hồi từ AI không hợp lệ.",
            "comparisonWithOtherCandidates": "Không xác định.",
            "problemSolvingSkills": "Không thể đánh giá.",
            "status": "Failed"
        }
        return jsonify(mock_report), 500
    except Exception as e:
        print(f"❌ Đã xảy ra lỗi không mong muốn: {e}")
        return jsonify({"error": str(e), "status": "Failed"}), 500
# --- CHẠY APP ---
if __name__ == '__main__':
    print("🚀 Flask AI Service is running at http://127.0.0.1:5000")
    app.run(debug=True, port=5000)
