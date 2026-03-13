# Test Scenarios: UserController.ApplyForStaff

## UTC_UserController_ApplyForStaff_01 - Positive
**Objective:** Verify that user can successfully Apply For Staff.

### Steps:
1. Navigate to the Apply For Staff interface.
2. Input valid required data for Apply For Staff.
3. Click the submit/action button.

### Expected Results:
1. System processes the request successfully.
2. Success message is displayed.
3. Data is updated/created in the database correctly.

---

## UTC_UserController_ApplyForStaff_02 - Negative
**Objective:** Verify system behavior when mandatory fields for Apply For Staff are missing.

### Steps:
1. Navigate to the Apply For Staff interface.
2. Leave mandatory fields empty.
3. Click the submit/action button.

### Expected Results:
1. System prevents submission.
2. Validation error messages are displayed for missing fields.

---

## UTC_UserController_ApplyForStaff_04 - Security
**Objective:** Verify that unauthorized users cannot Apply For Staff.

### Steps:
1. Attempt to call this endpoint without a valid token.

### Expected Results:
1. System returns 401 Unauthorized or 403 Forbidden.
2. No data is modified.

---

