# app.py

import os
import json
import random
from flask import Flask, request, jsonify
from dotenv import load_dotenv

# --- CLIENT CỦA CÁC NHÀ CUNG CẤP AI ---
import google.generativeai as genai
from perplexity import Perplexity # Nhập thư viện Perplexity

# --- KHỞI TẠO VÀ CẤU HÌNH ---

# Tải biến môi trường từ file .env (nếu có)
load_dotenv()

# Tạo ứng dụng Flask
app = Flask(__name__)

# --- CẤU HÌNH CHO GOOGLE GEMINI ---
try:
    gemini_api_key = os.environ.get("GOOGLE_API_KEY")
    if not gemini_api_key:
        raise ValueError("Lỗi: Biến môi trường GOOGLE_API_KEY chưa được thiết lập.")
    genai.configure(api_key=gemini_api_key)
    
    # Cấu hình mô hình Gemini 1.5 Flash
    gemini_generation_config = {
      "temperature": 1,
      "top_p": 0.95,
      "top_k": 64,
      "max_output_tokens": 8192,
      "response_mime_type": "application/json",
    }
    gemini_model = genai.GenerativeModel(
      model_name="gemini-1.5-flash-latest",
      generation_config=gemini_generation_config
    )
    print("✅ Cấu hình Google Gemini thành công.")
except Exception as e:
    print(f"🚨 Lỗi khi cấu hình Google Gemini: {e}")
    gemini_model = None

# --- CẤU HÌNH CHO PERPLEXITY ---
try:
    # Client Perplexity sẽ tự động đọc key từ biến môi trường PERPLEXITY_API_KEY
    perplexity_client = Perplexity()
    print("✅ Cấu hình Perplexity thành công.")
except Exception as e:
    print(f"🚨 Lỗi khi cấu hình Perplexity: {e}")
    perplexity_client = None

# --- CÁC ROUTE CỦA API ---

@app.route('/')
def index():
    """Hàm chào mừng khi truy cập vào trang chủ."""
    return "<h1>Chào mừng đến với API Phỏng vấn AI (Gemini + Perplexity)!</h1>"

@app.route('/api/generate_question', methods=['POST'])
def generate_question_api():
    """
    API endpoint để tạo câu hỏi phỏng vấn, sử dụng Google Gemini 1.5 Flash.
    """
    if not gemini_model:
        return jsonify({"error": "Dịch vụ Gemini chưa được cấu hình đúng."}), 503

    try:
        data = request.get_json()
    except Exception:
        return jsonify({"error": "Request body không phải là JSON hợp lệ."}), 400

    job_position = data.get("jobPosition", "developer")
    topic = data.get("topic", "Software Engineering")
    industry = data.get("industry", "IT")

    print(f"📩 [Gemini] Nhận yêu cầu tạo câu hỏi cho '{job_position}'")

    prompt = f"""
    Bạn là một chuyên gia tuyển dụng kỹ thuật. Hãy tạo ra MỘT câu hỏi phỏng vấn DUY NHẤT.
    Thông tin ứng viên:
    - Vị trí: {job_position}
    - Chủ đề chuyên môn: {topic}
    - Lĩnh vực công ty: {industry}
    Yêu cầu quan trọng: Chỉ trả về một đối tượng JSON duy nhất có cấu trúc sau:
    {{
      "questionText": "Nội dung câu hỏi ở đây",
      "difficultyLevel": <một số từ 1 (dễ) đến 3 (khó)>
    }}
    """

    try:
        response = gemini_model.generate_content(prompt)
        ai_json_data = json.loads(response.text)
        final_response = {
            "questionId": random.randint(1000, 9999),
            "questionText": ai_json_data.get("questionText"),
            "difficultyLevel": ai_json_data.get("difficultyLevel")
        }
        print(f"✅ [Gemini] Tạo câu hỏi thành công.")
        return jsonify(final_response), 200
    except Exception as e:
        print(f"🚨 [Gemini] Lỗi: {str(e)}")
        return jsonify({"error": f"Đã xảy ra lỗi không mong muốn với Gemini: {str(e)}"}), 500


@app.route('/api/generate_report', methods=['POST'])
def generate_report_api():
    """
    API endpoint để tạo báo cáo đánh giá, sử dụng Perplexity Pro (Reasoning).
    """
    if not perplexity_client:
        return jsonify({"error": "Dịch vụ Perplexity chưa được cấu hình đúng."}), 503
        
    try:
        data = request.get_json()
    except Exception:
        return jsonify({"error": "Request body không phải là JSON hợp lệ."}), 400

    question_text = data.get("questionText", "Không có câu hỏi")
    answer_text = data.get("answerText", "")

    print(f"🧠 [Perplexity] Bắt đầu tạo báo cáo phân tích...")

    # Với Perplexity, chúng ta truyền message dưới dạng một danh sách các "role"
    messages = [
        {
            "role": "system",
            "content": (
                "Bạn là một quản lý nhân sự và chuyên gia kỹ thuật cực kỳ kinh nghiệm. "
                "Nhiệm vụ của bạn là phân tích câu trả lời của ứng viên và trả về một đối tượng JSON duy nhất, "
                "không có bất kỳ giải thích hay markdown nào khác."
            ),
        },
        {
            "role": "user",
            "content": f"""
            Hãy phân tích và đánh giá câu trả lời phỏng vấn của ứng viên một cách khách quan.

            Bối cảnh phỏng vấn:
            - Câu hỏi: "{question_text}"
            - Câu trả lời của ứng viên: "{answer_text}"

            Hãy trả về một đối tượng JSON có cấu trúc chính xác như sau, hãy điền nội dung đánh giá của bạn vào các trường:
            {{
                "overallAssessment": "Đánh giá tổng quan về phần trả lời",
                "facialExpression": "Dựa trên văn bản, suy đoán về sự tự tin, biểu cảm",
                "speakingSpeedClarity": "Dựa trên văn phong, nhận xét về độ rõ ràng, mạch lạc",
                "expertiseExperience": "Đánh giá mức độ chuyên môn và kinh nghiệm",
                "responseDurationPerQuestion": "Ước lượng thời gian trả lời phù hợp, ví dụ: 'Khoảng 45s'",
                "answerContentAnalysis": "Phân tích sâu về nội dung câu trả lời",
                "comparisonWithOtherCandidates": "So sánh với mặt bằng chung",
                "problemSolvingSkills": "Đánh giá kỹ năng giải quyết vấn đề",
                "status": "Completed"
            }}
            """,
        },
    ]

    try:
        # Gọi API của Perplexity
        response = perplexity_client.chat.completions.create(
            model="llama-3-sonar-large-32k-chat", # Một model mạnh mẽ của Perplexity
            messages=messages,
        )
        
        # Lấy nội dung text từ response và parse thành JSON
        ai_response_text = response.choices[0].message.content
        ai_report = json.loads(ai_response_text)

        print("✅ [Perplexity] Tạo báo cáo thành công.")
        return jsonify(ai_report), 200

    except Exception as e:
        print(f"🚨 [Perplexity] Lỗi: {str(e)}")
        # In thêm response thô nếu có lỗi để dễ debug
        raw_response = getattr(e, 'response', None)
        if raw_response:
            print(f"Raw Perplexity Response: {raw_response.text}")
        return jsonify({"error": f"Đã xảy ra lỗi không mong muốn với Perplexity: {str(e)}"}), 500


# --- CHẠY ỨNG DỤNG ---

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=5001, debug=True)