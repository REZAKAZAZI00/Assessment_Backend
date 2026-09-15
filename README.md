# Assessment Backend 📚

بک‌اند سامانه مدیریت تکالیف و ارزیابی دانشجویی — یک REST API با ASP.NET Core 8 که فرآیند ایجاد کلاس، انتشار تکلیف، ارسال پاسخ توسط دانشجو و ثبت نمره را پوشش می‌دهد.

## ✨ قابلیت‌ها

- **احراز هویت JWT** — ورود و ثبت‌نام (معلم / دانشجو) با توکن Bearer
- **مدیریت کلاس‌ها** — ساخت، ویرایش، حذف کلاس و ترم‌های تحصیلی + عضویت دانشجو از طریق **لینک اشتراک‌گذاری** تولیدشده به‌صورت خودکار
- **مدیریت تکالیف (Assessment)** — ایجاد تکلیف با فایل پیوست، دریافت لیست ارسال‌ها و ثبت نمره
- **ارسال تکلیف توسط دانشجو** — آپلود فایل پاسخ به **Object Storage آروان کلود (S3 Compatible)**
- **گزارش و آمار** — گزارش عملکرد کلاس + آمار جداگانه برای معلم و دانشجو
- **مدیریت پایه تحصیلی (Grade)**

## 🏗️ معماری

پروژه به‌صورت سه‌لایه (N-Tier) سازمان‌دهی شده است:

```
Assessment_Backend.sln
│
├── Assessment_Backend            ← لایه Presentation (Web API)
│   ├── Controllers/              ← کنترلرها (Account, Course, Assessment, Grade, Statistics)
│   ├── Middleware/               ← لاگ Request/Response و مدیریت سراسری Exception
│   └── Filters/                  ← ServiceResultFilter و ModelValidationFilter
│
├── Assessment_Backend.Core       ← لایه Business Logic
│   ├── Services/                 ← سرویس‌ها + اینترفیس‌های آن‌ها
│   ├── DTOs/                     ← مدل‌های ورودی/خروجی
│   ├── Security/                 ← رمزنگاری و توکن
│   ├── Generator/                ← تولید لینک و نام یکتا
│   └── Exceptions/  Convertors/
│
└── Assessment_Backend.DataLayer  ← لایه دسترسی به داده
    ├── Context/                  ← AssessmentDbContext (EF Core)
    ├── Entities/                 ← User, Teacher, Student, Course, Assessment, Sub, ...
    └── Migrations/
```

## 🛠️ تکنولوژی‌ها

| مورد | نسخه / ابزار |
|---|---|
| فریم‌ورک | ASP.NET Core 8.0 |
| ORM | Entity Framework Core 9 (SQL Server) |
| احراز هویت | JWT Bearer |
| لاگینگ | Serilog (Console + فایل روزانه + SQL Server) |
| مستندسازی | Swagger / OpenAPI |
| Object Storage | آروان کلود (S3 Compatible) |
| کانتینر | Docker (Linux) |

## 🔌 خلاصه API

| Controller | مسیر پایه | توضیح |
|---|---|---|
| `AccountController` | `/api/accounts` | ورود (`login`)، ثبت‌نام معلم و دانشجو |
| `CourseController` | `/api/courses` | ترم‌ها، CRUD کلاس، عضویت/خروج با لینک (`joinclass` / `leavingclass`) |
| `AssessmentController` | `/api/assessments` | CRUD تکلیف، ارسال پاسخ، ثبت نمره، گزارش |
| `GradeController` | `/api/grades` | لیست پایه‌های تحصیلی |
| `StatisticsController` | `/api/statistics` | آمار دانشجو و معلم |

> مستندات کامل و قابل اجرا در مسیر **Swagger UI** (مسیر ریشه برنامه) موجود است؛ احراز هویت Bearer در Swagger پیکربندی شده است.

## 🚀 راه‌اندازی

### پیش‌نیازها

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server
- (اختیاری) Docker

### تنظیمات

مقادیر لازم در `appsettings.json` (یا بهتر: User Secrets / متغیرهای محیطی):

```json
{
  "ConnectionStrings": {
    "AssessmentConnection": "Data Source=<server>;Initial Catalog=<db>;User Id=<user>;Password=<pass>;TrustServerCertificate=True;"
  },
  "Authentication": {
    "SecretForKey": "<کلید متقارن JWT>",
    "Issuer": "https://<your-domain>",
    "Audience": "assessment"
  },
  "S3Storage": {
    "ServiceUrl": "https://s3.<region>.arvanstorage.ir",
    "AccessKey": "<access-key>",
    "SecretKey": "<secret-key>",
    "BucketName": "assessment"
  }
}
```

> ⚠️ **نکته امنیتی:** مقادیر واقعی `SecretForKey`، رمز دیتابیس و کلیدهای آروان را در ریپازیتوری Commit نکنید. در محیط توسعه از `dotnet user-secrets` و در محیط اجرا از متغیرهای محیطی یا Secret Manager استفاده کنید.

### اجرای محلی

```bash
dotnet restore
dotnet run --project Assessment_Backend
```

Migration های در انتظار به‌صورت خودکار در زمان استارتاپ اعمال می‌شوند؛ نیازی به اجرای دستی `dotnet ef database update` نیست.

### اجرا با Docker

```bash
docker build -t assessment-backend .
docker run -d -p 8080:8080 -p 8081:8081 assessment-backend
```

## 📝 لاگینگ

هر درخواست HTTP توسط `RequestResponseLoggingMiddleware` در دو جدول دیتابیس ثبت می‌شود:

- `RequestLogs` — جزئیات درخواست‌های ورودی (`LogType=Request`)
- `ResponseLogs` — جزئیات پاسخ‌ها (`LogType=Response`)

علاوه بر آن، Serilog خروجی را روی **Console** و در فایل روزانه `data/backend.txt` نیز می‌نویسد. جدول‌های لاگ در اولین اجرا به‌صورت خودکار ساخته می‌شوند (`AutoCreateSqlTable`).

خطاهای سراسری برنامه تنها در یک نقطه (Middleware مدیریت Exception) هندل می‌شوند و پاسخ استاندارد برمی‌گردانند.

## 📄 مجوز

این پروژه به‌عنوان تکلیف دانشجویی تهیه شده است.
