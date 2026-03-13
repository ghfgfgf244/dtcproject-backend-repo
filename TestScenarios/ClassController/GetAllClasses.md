# Test Scenarios: ClassController.GetAllClasses

## UTC_ClassController_GetAllClasses_01 - Positive
**Objective:** Verify that user can successfully Get All Classes.

### Steps:
1. Navigate to the Get All Classes interface.
2. Input valid required data for Get All Classes.
3. Click the submit/action button.

### Expected Results:
1. System processes the request successfully.
2. Success message is displayed.
3. Data is updated/created in the database correctly.

---

## UTC_ClassController_GetAllClasses_02 - Negative
**Objective:** Verify system behavior when mandatory fields for Get All Classes are missing.

### Steps:
1. Navigate to the Get All Classes interface.
2. Leave mandatory fields empty.
3. Click the submit/action button.

### Expected Results:
1. System prevents submission.
2. Validation error messages are displayed for missing fields.

---

## UTC_ClassController_GetAllClasses_04 - Security
**Objective:** Verify that unauthorized users cannot Get All Classes.

### Steps:
1. Attempt to call this endpoint without a valid token.

### Expected Results:
1. System returns 401 Unauthorized or 403 Forbidden.
2. No data is modified.

---

