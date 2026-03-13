# API Automation with Playwright

Thư mục này chứa các script automation test cho Backend API.

## Hướng dẫn cài đặt

Để chạy các test này, bạn cần có **Node.js** và cài đặt Playwright:

1. Di chuyển vào thư mục này:
   ```bash
   cd tests/API
   ```

2. Init và Install Playwright (nếu chưa có):
   ```bash
   npm init -y
   npm i -D @playwright/test
   ```

## Cách chạy Test

1. Đảm bảo Backend đã được chạy (`dotnet run` trong project `dtc.API`).
2. Chạy test script:
   ```bash
   npx playwright test
   ```

## Danh sách Test hiện có

1. `auth.test.js`: Luồng Register -> Login -> Get Profile.
