# Test Scenarios: AttendanceController.GetAttendanceBySchedule

## UTC_AttendanceController_GetAttendanceBySchedule_01 - Positive
**Objective:** Verify that user can successfully Get Attendance By Schedule.

### Steps:
1. Navigate to the Get Attendance By Schedule interface.
2. Input valid required data for Get Attendance By Schedule.
3. Click the submit/action button.

### Expected Results:
1. System processes the request successfully.
2. Success message is displayed.
3. Data is updated/created in the database correctly.

---

## UTC_AttendanceController_GetAttendanceBySchedule_02 - Negative
**Objective:** Verify system behavior when mandatory fields for Get Attendance By Schedule are missing.

### Steps:
1. Navigate to the Get Attendance By Schedule interface.
2. Leave mandatory fields empty.
3. Click the submit/action button.

### Expected Results:
1. System prevents submission.
2. Validation error messages are displayed for missing fields.

---

## UTC_AttendanceController_GetAttendanceBySchedule_04 - Security
**Objective:** Verify that unauthorized users cannot Get Attendance By Schedule.

### Steps:
1. Attempt to call this endpoint without a valid token.

### Expected Results:
1. System returns 401 Unauthorized or 403 Forbidden.
2. No data is modified.

---

