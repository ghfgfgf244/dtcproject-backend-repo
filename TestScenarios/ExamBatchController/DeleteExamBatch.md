# Test Scenarios: ExamBatchController.DeleteExamBatch

## UTC_ExamBatchController_DeleteExamBatch_01 - Positive
**Objective:** Verify that user can successfully Delete Exam Batch.

### Steps:
1. Navigate to the Delete Exam Batch interface.
2. Input valid required data for Delete Exam Batch.
3. Click the submit/action button.

### Expected Results:
1. System processes the request successfully.
2. Success message is displayed.
3. Data is updated/created in the database correctly.

---

## UTC_ExamBatchController_DeleteExamBatch_02 - Negative
**Objective:** Verify system behavior when mandatory fields for Delete Exam Batch are missing.

### Steps:
1. Navigate to the Delete Exam Batch interface.
2. Leave mandatory fields empty.
3. Click the submit/action button.

### Expected Results:
1. System prevents submission.
2. Validation error messages are displayed for missing fields.

---

## UTC_ExamBatchController_DeleteExamBatch_04 - Security
**Objective:** Verify that unauthorized users cannot Delete Exam Batch.

### Steps:
1. Attempt to call this endpoint without a valid token.

### Expected Results:
1. System returns 401 Unauthorized or 403 Forbidden.
2. No data is modified.

---

