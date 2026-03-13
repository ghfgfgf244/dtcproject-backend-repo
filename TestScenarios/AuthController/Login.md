# Test Scenarios: AuthController.Login

## UTC_AuthController_Login_01 - Positive
**Objective:** Verify that user can successfully Login.

### Steps:
1. Navigate to the Login interface.
2. Input valid required data for Login.
3. Click the submit/action button.

### Expected Results:
1. System processes the request successfully.
2. Success message is displayed.
3. Data is updated/created in the database correctly.

---

## UTC_AuthController_Login_02 - Negative
**Objective:** Verify system behavior when mandatory fields for Login are missing.

### Steps:
1. Navigate to the Login interface.
2. Leave mandatory fields empty.
3. Click the submit/action button.

### Expected Results:
1. System prevents submission.
2. Validation error messages are displayed for missing fields.

---

## UTC_AuthController_Login_03 - Negative
**Objective:** Verify system behavior with invalid data format (e.g., invalid email).

### Steps:
1. Input data with invalid format (e.g., 'invalid-email').
2. Click the submit/action button.

### Expected Results:
1. System shows 'Invalid Format' error message.
2. Request is not processed.

---

## UTC_AuthController_Login_04 - Security
**Objective:** Verify that unauthorized users cannot Login.

### Steps:
1. Attempt to call this endpoint without a valid token.

### Expected Results:
1. System returns 401 Unauthorized or 403 Forbidden.
2. No data is modified.

---

