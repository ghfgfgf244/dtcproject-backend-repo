# Test Scenarios: AttendanceController.MarkAttendance

## UTC_AttendanceController_MarkAttendance_01 - Positive
**Objective:** Verify that user can successfully Mark Attendance.

### Steps:
1. Navigate to the Mark Attendance interface.
2. Input valid required data for Mark Attendance.
3. Click the submit/action button.

### Expected Results:
1. System processes the request successfully.
2. Success message is displayed.
3. Data is updated/created in the database correctly.

---

## UTC_AttendanceController_MarkAttendance_02 - Negative
**Objective:** Verify system behavior when mandatory fields for Mark Attendance are missing.

### Steps:
1. Navigate to the Mark Attendance interface.
2. Leave mandatory fields empty.
3. Click the submit/action button.

### Expected Results:
1. System prevents submission.
2. Validation error messages are displayed for missing fields.

---

## UTC_AttendanceController_MarkAttendance_04 - Security
**Objective:** Verify that unauthorized users cannot Mark Attendance.

### Steps:
1. Attempt to call this endpoint without a valid token.

### Expected Results:
1. System returns 401 Unauthorized or 403 Forbidden.
2. No data is modified.

---

