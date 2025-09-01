# 🌐 Web Pages Downloader

A simple **C# console application** that asynchronously downloads multiple web pages using `HttpClient`.  
It demonstrates the use of **async/await**, **CancellationToken**, and **task coordination** with `Task.WhenAny`.

---

## 🚀 Features
- Download a list of predefined URLs asynchronously.
- Cancel downloading at any time by pressing the **SPACEBAR**.
- Graceful cancellation handling using `CancellationTokenSource`.
- Displays results in a table with:
  - **URL**
  - **Character count** of the page
  - **Success/Failure status**
- Handles different types of errors:
  - HTTP errors
  - Timeout
  - Canceled operations
  - General exceptions

---

## 🛠️ Technologies
- **C# 12 / .NET 9**
- `HttpClient`
- `async/await`
- `CancellationTokenSource`
- `Task.WhenAny`
- Console I/O


---

## 📊 Example Output
<img width="1045" height="439" alt="image" src="https://github.com/user-attachments/assets/4589df44-505c-44de-aed3-cb483f255be9" />


---

## How It Works

1. The application starts downloading a predefined list of websites.
2. Press **SPACEBAR** at any time to stop further downloads.
3. After completion (or cancellation), results are shown in a formatted table.
