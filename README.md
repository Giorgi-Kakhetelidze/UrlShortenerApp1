# UrlShortenerApp1

## 🌐 URL Shortener API

A modern, scalable URL shortening service built with **C# 13** and **.NET 9**, using **Apache Cassandra** for persistent, distributed storage.

---

## 🚀 Features

- 🔗 Shorten URLs with optional custom aliases  
- 🚀 Redirect to original URLs via short codes  
- ⏳ Set expiration dates for links  
- 📊 Track click counts for basic analytics  
- 🔧 Full CRUD operations on short URLs  
- 💾 Apache Cassandra integration for scalability and high availability  

---

## 🛠️ Tech Stack

- **.NET 9 / C# 13**
- **Apache Cassandra** (via DataStax C# Driver)
- **Docker + Docker Compose**
- **RESTful API**

---

## 📦 Getting Started

### ⚙️ Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/)
- [Docker Compose](https://docs.docker.com/compose/)

---

### 🐳 Docker Setup

> Run the project using Docker Compose (Recommended for local setup)

1. Navigate to the source folder:

   ```bash
   cd src
   ```

2. Build the containers:

   ```bash
   docker compose build
   ```

3. Run the containers:

   ```bash
   docker compose up
   ```

4. The API will be available at:

   ```
   http://localhost:5000
   ```

---

### 🧑‍💻 Manual Setup (Without Docker)

1. **Clone the repository:**

    ```bash
    git clone https://github.com/yourusername/url-shortener-api.git
    cd url-shortener-api
    ```

2. **Configure Cassandra:**

    - Update your `appsettings.json` or environment variables with the correct:
      - Cassandra contact point(s)
      - Port
      - Keyspace

3. **Restore .NET dependencies:**

    ```bash
    dotnet restore
    ```

4. **Ensure the required Cassandra table exists:**

    ```sql
    CREATE TABLE url_shortener.urls (
        short_code text PRIMARY KEY,
        original_url text,
        created_at timestamp,
        expiration_date timestamp,
        click_count int,
        is_active boolean
    );
    ```

5. **Run the API manually:**

    ```bash
    dotnet run --project UrlShortenerApp1/src/UrlShortener.Api
    ```

---

## 📚 API Endpoints

| Method | Endpoint                  | Description                        |
|--------|---------------------------|------------------------------------|
| POST   | `/api/urls`               | Create a new short URL             |
| GET    | `/api/urls/{shortCode}`   | Retrieve original URL              |
| PUT    | `/api/urls/{shortCode}`   | Update an existing short URL       |
| DELETE | `/api/urls/{shortCode}`   | Delete a short URL                 |
| GET    | `/api/urls`               | List all short URLs                |

---

### 🔧 Example: Create Short URL

**Request:**

```http
POST /api/urls
Content-Type: application/json
```

```json
{
  "originalUrl": "https://example.com/very/long/link",
  "customAlias": "my-custom-code",       // optional
  "expirationDate": "2025-12-31T23:59:59"
}
```

**Response:**

```json
{
  "shortCode": "my-custom-code",
  "shortUrl": "http://localhost:5000/my-custom-code",
  "clickCount": 0,
  "isActive": true,
  "expirationDate": "2025-12-31T23:59:59"
}
```

---

## 🧾 License

This project is open-source and available under the [MIT License](LICENSE).

---

## 📬 Contact

For issues, suggestions, or contributions, please open an issue or pull request.

