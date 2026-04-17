# 🚨 Disaster Prediction and Alert System API

โปรเจกต์ API สำหรับติดตามและแจ้งเตือนภัยพิบัติ โดยคำนวณความเสี่ยง (Risk Score) จากข้อมูลสภาพอากาศและสถิติแผ่นดินไหวแบบ Real-time พร้อมระบบส่งอีเมลแจ้งเตือนเมื่อระดับความเสี่ยงอยู่ในเกณฑ์อันตราย

---

## 🛠 Tech Stack

| Component | Technology |
| :--- | :--- |
| **Framework** | .NET 10 (C#) |
| **Database** | PostgreSQL ([Neon.tech](https://neon.tech/)) |
| **Cache** | Redis ([Upstash](https://upstash.com/)) |
| **Email SDK** | SendGrid |
| **Infrastructure** | Azure App Service |
| **Monitoring** | Azure Application Insights |
| **External Data** | OpenWeather API, USGS Earthquake Data |

---

## 🚀 API Endpoints

### 📍 Region & Settings
* `POST /api/regions`
    * **Description:** ผู้ใช้เพิ่มพื้นที่ด้วย พิกัด ละติจูดและลองจิจูด และประเภทภัยพิบัติของการแจ้งเตือนที่ต้องการ
* `POST /api/alert-settings`
    * **Description:** ผู้ใช้สามารถตั้งค่า threshold เพื่อใช้ในการเปรียบเทียบกับ risk score

### ⛈ Disaster Monitoring
* `GET /api/disaster-risks`
    * **Process:**
        1. ดึงข้อมูลล่าสุดจาก OpenWeather และ USGS
        2. คำนวณ **Risk Score** และแบ่งระดับ **Risk Level** (Low, Medium, High)
        3. บันทึกผลลัพธ์ลง **Redis Cache** (TTL 15 นาที)
* `GET /api/alerts`
    * **Description:** แสดงรายการแจ้งเตือนล่าสุดของแต่ละพื้นที่ที่บันทึกไว้

### 📧 Notification
* `POST /api/alerts/send`
    * **Process:** 
        1. ตรวจสอบพื้นที่ที่มีความเสี่ยงระดับ **High**
        2. ส่ง Email แจ้งเตือนผ่าน SendGrid ให้ผู้ใช้ที่ติดตามข้อมูลแต่ละพื้นที่