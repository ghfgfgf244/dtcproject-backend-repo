# Test Scenarios: ResourceLearningController.DeleteResourceLearning

## UTC_ResourceLearningController_DeleteResourceLearning_01 - Positive
**Objective:** Verify that user can successfully Delete Resource Learning.

### Steps:
1. Navigate to the Delete Resource Learning interface.
2. Input valid required data for Delete Resource Learning.
3. Click the submit/action button.

### Expected Results:
1. System processes the request successfully.
2. Success message is displayed.
3. Data is updated/created in the database correctly.

---

## UTC_ResourceLearningController_DeleteResourceLearning_02 - Negative
**Objective:** Verify system behavior when mandatory fields for Delete Resource Learning are missing.

### Steps:
1. Navigate to the Delete Resource Learning interface.
2. Leave mandatory fields empty.
3. Click the submit/action button.

### Expected Results:
1. System prevents submission.
2. Validation error messages are displayed for missing fields.

---

## UTC_ResourceLearningController_DeleteResourceLearning_04 - Security
**Objective:** Verify that unauthorized users cannot Delete Resource Learning.

### Steps:
1. Attempt to call this endpoint without a valid token.

### Expected Results:
1. System returns 401 Unauthorized or 403 Forbidden.
2. No data is modified.

---

