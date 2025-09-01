# 🌐 Web Pages Downloader

A simple C# console application that asynchronously loads multiple web pages in parallel using `HttpClient` and `async/await`.

---

## 🚀 Features
- Asynchronous web page downloading.
- Parallel requests with `Task.WhenAll`.
- Error handling (`HttpRequestException`, `Timeout`, general exceptions).
- Displays:
  - URL
  - Number of characters in the response
  - Status (success or failure)
- Summary of total successful downloads and total characters loaded.

---

## 🛠️ Technologies
- **C# 12 / .NET 9**
- `HttpClient`
- `async/await`
- `Task.WhenAll`

---

## 📊 Example Output
<img width="988" height="819" alt="image" src="https://github.com/user-attachments/assets/1af6d4de-6869-414e-bf95-530904d11f40" />
