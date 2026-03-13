# Test Scenarios: CollaboratorController.GenerateReferralCode

## UTC_CollaboratorController_GenerateReferralCode_01 - Positive
**Objective:** Verify that user can successfully Generate Referral Code.

### Steps:
1. Navigate to the Generate Referral Code interface.
2. Input valid required data for Generate Referral Code.
3. Click the submit/action button.

### Expected Results:
1. System processes the request successfully.
2. Success message is displayed.
3. Data is updated/created in the database correctly.

---

## UTC_CollaboratorController_GenerateReferralCode_02 - Negative
**Objective:** Verify system behavior when mandatory fields for Generate Referral Code are missing.

### Steps:
1. Navigate to the Generate Referral Code interface.
2. Leave mandatory fields empty.
3. Click the submit/action button.

### Expected Results:
1. System prevents submission.
2. Validation error messages are displayed for missing fields.

---

## UTC_CollaboratorController_GenerateReferralCode_04 - Security
**Objective:** Verify that unauthorized users cannot Generate Referral Code.

### Steps:
1. Attempt to call this endpoint without a valid token.

### Expected Results:
1. System returns 401 Unauthorized or 403 Forbidden.
2. No data is modified.

---

