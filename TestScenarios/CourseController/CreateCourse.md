# Test Scenarios: CourseController.CreateCourse

## UTC_CourseController_CreateCourse_01 - Positive
**Objective:** Verify that user can successfully Create Course.

### Steps:
1. Navigate to the Create Course interface.
2. Input valid required data for Create Course.
3. Click the submit/action button.

### Expected Results:
1. System processes the request successfully.
2. Success message is displayed.
3. Data is updated/created in the database correctly.

---

## UTC_CourseController_CreateCourse_02 - Negative
**Objective:** Verify system behavior when mandatory fields for Create Course are missing.

### Steps:
1. Navigate to the Create Course interface.
2. Leave mandatory fields empty.
3. Click the submit/action button.

### Expected Results:
1. System prevents submission.
2. Validation error messages are displayed for missing fields.

---

## UTC_CourseController_CreateCourse_04 - Security
**Objective:** Verify that unauthorized users cannot Create Course.

### Steps:
1. Attempt to call this endpoint without a valid token.

### Expected Results:
1. System returns 401 Unauthorized or 403 Forbidden.
2. No data is modified.

---

