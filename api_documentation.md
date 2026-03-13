# dtc.API
## Version: 1.0

---

### [POST] /api/Attendance
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [MarkAttendanceRequestDto](#markattendancerequestdto)<br>**text/json**: [MarkAttendanceRequestDto](#markattendancerequestdto)<br>**application/*+json**: [MarkAttendanceRequestDto](#markattendancerequestdto)<br> | **application/json**: [MarkAttendanceRequestDto](#markattendancerequestdto)<br>**text/json**: [MarkAttendanceRequestDto](#markattendancerequestdto)<br>**application/*+json**: [MarkAttendanceRequestDto](#markattendancerequestdto)<br> | **application/json**: [MarkAttendanceRequestDto](#markattendancerequestdto)<br>**text/json**: [MarkAttendanceRequestDto](#markattendancerequestdto)<br>**application/*+json**: [MarkAttendanceRequestDto](#markattendancerequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Attendance/Schedule/{classScheduleId}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| classScheduleId | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Attendance/Report/Class/{classId}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| classId | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/Auth/register
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [RegisterRequestDto](#registerrequestdto)<br>**text/json**: [RegisterRequestDto](#registerrequestdto)<br>**application/*+json**: [RegisterRequestDto](#registerrequestdto)<br> | **application/json**: [RegisterRequestDto](#registerrequestdto)<br>**text/json**: [RegisterRequestDto](#registerrequestdto)<br>**application/*+json**: [RegisterRequestDto](#registerrequestdto)<br> | **application/json**: [RegisterRequestDto](#registerrequestdto)<br>**text/json**: [RegisterRequestDto](#registerrequestdto)<br>**application/*+json**: [RegisterRequestDto](#registerrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [POST] /api/Auth/login
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [LoginRequestDto](#loginrequestdto)<br>**text/json**: [LoginRequestDto](#loginrequestdto)<br>**application/*+json**: [LoginRequestDto](#loginrequestdto)<br> | **application/json**: [LoginRequestDto](#loginrequestdto)<br>**text/json**: [LoginRequestDto](#loginrequestdto)<br>**application/*+json**: [LoginRequestDto](#loginrequestdto)<br> | **application/json**: [LoginRequestDto](#loginrequestdto)<br>**text/json**: [LoginRequestDto](#loginrequestdto)<br>**application/*+json**: [LoginRequestDto](#loginrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [POST] /api/Auth/logout
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/Center
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateCenterRequestDto](#createcenterrequestdto)<br>**text/json**: [CreateCenterRequestDto](#createcenterrequestdto)<br>**application/*+json**: [CreateCenterRequestDto](#createcenterrequestdto)<br> | **application/json**: [CreateCenterRequestDto](#createcenterrequestdto)<br>**text/json**: [CreateCenterRequestDto](#createcenterrequestdto)<br>**application/*+json**: [CreateCenterRequestDto](#createcenterrequestdto)<br> | **application/json**: [CreateCenterRequestDto](#createcenterrequestdto)<br>**text/json**: [CreateCenterRequestDto](#createcenterrequestdto)<br>**application/*+json**: [CreateCenterRequestDto](#createcenterrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Center
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/Center/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateCenterRequestDto](#updatecenterrequestdto)<br>**text/json**: [UpdateCenterRequestDto](#updatecenterrequestdto)<br>**application/*+json**: [UpdateCenterRequestDto](#updatecenterrequestdto)<br> | **application/json**: [UpdateCenterRequestDto](#updatecenterrequestdto)<br>**text/json**: [UpdateCenterRequestDto](#updatecenterrequestdto)<br>**application/*+json**: [UpdateCenterRequestDto](#updatecenterrequestdto)<br> | **application/json**: [UpdateCenterRequestDto](#updatecenterrequestdto)<br>**text/json**: [UpdateCenterRequestDto](#updatecenterrequestdto)<br>**application/*+json**: [UpdateCenterRequestDto](#updatecenterrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [DELETE] /api/Center/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Center/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [POST] /api/Center/{id}/users
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [AssignUsersRequestDto](#assignusersrequestdto)<br>**text/json**: [AssignUsersRequestDto](#assignusersrequestdto)<br>**application/*+json**: [AssignUsersRequestDto](#assignusersrequestdto)<br> | **application/json**: [AssignUsersRequestDto](#assignusersrequestdto)<br>**text/json**: [AssignUsersRequestDto](#assignusersrequestdto)<br>**application/*+json**: [AssignUsersRequestDto](#assignusersrequestdto)<br> | **application/json**: [AssignUsersRequestDto](#assignusersrequestdto)<br>**text/json**: [AssignUsersRequestDto](#assignusersrequestdto)<br>**application/*+json**: [AssignUsersRequestDto](#assignusersrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/Class
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateClassRequestDto](#createclassrequestdto)<br>**text/json**: [CreateClassRequestDto](#createclassrequestdto)<br>**application/*+json**: [CreateClassRequestDto](#createclassrequestdto)<br> | **application/json**: [CreateClassRequestDto](#createclassrequestdto)<br>**text/json**: [CreateClassRequestDto](#createclassrequestdto)<br>**application/*+json**: [CreateClassRequestDto](#createclassrequestdto)<br> | **application/json**: [CreateClassRequestDto](#createclassrequestdto)<br>**text/json**: [CreateClassRequestDto](#createclassrequestdto)<br>**application/*+json**: [CreateClassRequestDto](#createclassrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Class
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/Class/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateClassRequestDto](#updateclassrequestdto)<br>**text/json**: [UpdateClassRequestDto](#updateclassrequestdto)<br>**application/*+json**: [UpdateClassRequestDto](#updateclassrequestdto)<br> | **application/json**: [UpdateClassRequestDto](#updateclassrequestdto)<br>**text/json**: [UpdateClassRequestDto](#updateclassrequestdto)<br>**application/*+json**: [UpdateClassRequestDto](#updateclassrequestdto)<br> | **application/json**: [UpdateClassRequestDto](#updateclassrequestdto)<br>**text/json**: [UpdateClassRequestDto](#updateclassrequestdto)<br>**application/*+json**: [UpdateClassRequestDto](#updateclassrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Class/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [DELETE] /api/Class/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [POST] /api/Class/{id}/teachers
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [AssignTeachersRequestDto](#assignteachersrequestdto)<br>**text/json**: [AssignTeachersRequestDto](#assignteachersrequestdto)<br>**application/*+json**: [AssignTeachersRequestDto](#assignteachersrequestdto)<br> | **application/json**: [AssignTeachersRequestDto](#assignteachersrequestdto)<br>**text/json**: [AssignTeachersRequestDto](#assignteachersrequestdto)<br>**application/*+json**: [AssignTeachersRequestDto](#assignteachersrequestdto)<br> | **application/json**: [AssignTeachersRequestDto](#assignteachersrequestdto)<br>**text/json**: [AssignTeachersRequestDto](#assignteachersrequestdto)<br>**application/*+json**: [AssignTeachersRequestDto](#assignteachersrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [POST] /api/Class/{id}/students
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [AssignStudentsRequestDto](#assignstudentsrequestdto)<br>**text/json**: [AssignStudentsRequestDto](#assignstudentsrequestdto)<br>**application/*+json**: [AssignStudentsRequestDto](#assignstudentsrequestdto)<br> | **application/json**: [AssignStudentsRequestDto](#assignstudentsrequestdto)<br>**text/json**: [AssignStudentsRequestDto](#assignstudentsrequestdto)<br>**application/*+json**: [AssignStudentsRequestDto](#assignstudentsrequestdto)<br> | **application/json**: [AssignStudentsRequestDto](#assignstudentsrequestdto)<br>**text/json**: [AssignStudentsRequestDto](#assignstudentsrequestdto)<br>**application/*+json**: [AssignStudentsRequestDto](#assignstudentsrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [GET] /api/Collaborator/list
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Collaborator/token
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [POST] /api/Collaborator/token
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateReferralCodeRequestDto](#createreferralcoderequestdto)<br>**text/json**: [CreateReferralCodeRequestDto](#createreferralcoderequestdto)<br>**application/*+json**: [CreateReferralCodeRequestDto](#createreferralcoderequestdto)<br> | **application/json**: [CreateReferralCodeRequestDto](#createreferralcoderequestdto)<br>**text/json**: [CreateReferralCodeRequestDto](#createreferralcoderequestdto)<br>**application/*+json**: [CreateReferralCodeRequestDto](#createreferralcoderequestdto)<br> | **application/json**: [CreateReferralCodeRequestDto](#createreferralcoderequestdto)<br>**text/json**: [CreateReferralCodeRequestDto](#createreferralcoderequestdto)<br>**application/*+json**: [CreateReferralCodeRequestDto](#createreferralcoderequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Collaborator/token/usage
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Collaborator/commission/rate
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [POST] /api/Collaborator/commission/calculate
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Collaborator/commission
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/Course
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateCourseRequestDto](#createcourserequestdto)<br>**text/json**: [CreateCourseRequestDto](#createcourserequestdto)<br>**application/*+json**: [CreateCourseRequestDto](#createcourserequestdto)<br> | **application/json**: [CreateCourseRequestDto](#createcourserequestdto)<br>**text/json**: [CreateCourseRequestDto](#createcourserequestdto)<br>**application/*+json**: [CreateCourseRequestDto](#createcourserequestdto)<br> | **application/json**: [CreateCourseRequestDto](#createcourserequestdto)<br>**text/json**: [CreateCourseRequestDto](#createcourserequestdto)<br>**application/*+json**: [CreateCourseRequestDto](#createcourserequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/Course/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateCourseRequestDto](#updatecourserequestdto)<br>**text/json**: [UpdateCourseRequestDto](#updatecourserequestdto)<br>**application/*+json**: [UpdateCourseRequestDto](#updatecourserequestdto)<br> | **application/json**: [UpdateCourseRequestDto](#updatecourserequestdto)<br>**text/json**: [UpdateCourseRequestDto](#updatecourserequestdto)<br>**application/*+json**: [UpdateCourseRequestDto](#updatecourserequestdto)<br> | **application/json**: [UpdateCourseRequestDto](#updatecourserequestdto)<br>**text/json**: [UpdateCourseRequestDto](#updatecourserequestdto)<br>**application/*+json**: [UpdateCourseRequestDto](#updatecourserequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [DELETE] /api/Course/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Course/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Course/admin/all
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Course/available
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/CourseRegistration
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [RegisterCourseRequestDto](#registercourserequestdto)<br>**text/json**: [RegisterCourseRequestDto](#registercourserequestdto)<br>**application/*+json**: [RegisterCourseRequestDto](#registercourserequestdto)<br> | **application/json**: [RegisterCourseRequestDto](#registercourserequestdto)<br>**text/json**: [RegisterCourseRequestDto](#registercourserequestdto)<br>**application/*+json**: [RegisterCourseRequestDto](#registercourserequestdto)<br> | **application/json**: [RegisterCourseRequestDto](#registercourserequestdto)<br>**text/json**: [RegisterCourseRequestDto](#registercourserequestdto)<br>**application/*+json**: [RegisterCourseRequestDto](#registercourserequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/CourseRegistration/{id}/cancel
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CancelRegistrationRequestDto](#cancelregistrationrequestdto)<br>**text/json**: [CancelRegistrationRequestDto](#cancelregistrationrequestdto)<br>**application/*+json**: [CancelRegistrationRequestDto](#cancelregistrationrequestdto)<br> | **application/json**: [CancelRegistrationRequestDto](#cancelregistrationrequestdto)<br>**text/json**: [CancelRegistrationRequestDto](#cancelregistrationrequestdto)<br>**application/*+json**: [CancelRegistrationRequestDto](#cancelregistrationrequestdto)<br> | **application/json**: [CancelRegistrationRequestDto](#cancelregistrationrequestdto)<br>**text/json**: [CancelRegistrationRequestDto](#cancelregistrationrequestdto)<br>**application/*+json**: [CancelRegistrationRequestDto](#cancelregistrationrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/CourseRegistration/{id}/status
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateRegistrationStatusDto](#updateregistrationstatusdto)<br>**text/json**: [UpdateRegistrationStatusDto](#updateregistrationstatusdto)<br>**application/*+json**: [UpdateRegistrationStatusDto](#updateregistrationstatusdto)<br> | **application/json**: [UpdateRegistrationStatusDto](#updateregistrationstatusdto)<br>**text/json**: [UpdateRegistrationStatusDto](#updateregistrationstatusdto)<br>**application/*+json**: [UpdateRegistrationStatusDto](#updateregistrationstatusdto)<br> | **application/json**: [UpdateRegistrationStatusDto](#updateregistrationstatusdto)<br>**text/json**: [UpdateRegistrationStatusDto](#updateregistrationstatusdto)<br>**application/*+json**: [UpdateRegistrationStatusDto](#updateregistrationstatusdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/CourseRegistration/me
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/CourseRegistration/all
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/CourseRegistration/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [GET] /api/Dashboard/finance
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Dashboard/admission
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/Document
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateDocumentRequestDto](#createdocumentrequestdto)<br>**text/json**: [CreateDocumentRequestDto](#createdocumentrequestdto)<br>**application/*+json**: [CreateDocumentRequestDto](#createdocumentrequestdto)<br> | **application/json**: [CreateDocumentRequestDto](#createdocumentrequestdto)<br>**text/json**: [CreateDocumentRequestDto](#createdocumentrequestdto)<br>**application/*+json**: [CreateDocumentRequestDto](#createdocumentrequestdto)<br> | **application/json**: [CreateDocumentRequestDto](#createdocumentrequestdto)<br>**text/json**: [CreateDocumentRequestDto](#createdocumentrequestdto)<br>**application/*+json**: [CreateDocumentRequestDto](#createdocumentrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Document
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/Document/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateDocumentRequestDto](#updatedocumentrequestdto)<br>**text/json**: [UpdateDocumentRequestDto](#updatedocumentrequestdto)<br>**application/*+json**: [UpdateDocumentRequestDto](#updatedocumentrequestdto)<br> | **application/json**: [UpdateDocumentRequestDto](#updatedocumentrequestdto)<br>**text/json**: [UpdateDocumentRequestDto](#updatedocumentrequestdto)<br>**application/*+json**: [UpdateDocumentRequestDto](#updatedocumentrequestdto)<br> | **application/json**: [UpdateDocumentRequestDto](#updatedocumentrequestdto)<br>**text/json**: [UpdateDocumentRequestDto](#updatedocumentrequestdto)<br>**application/*+json**: [UpdateDocumentRequestDto](#updatedocumentrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [DELETE] /api/Document/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Document/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [POST] /api/Document/{id}/verify
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/Evaluation
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateStudentEvaluationRequestDto](#createstudentevaluationrequestdto)<br>**text/json**: [CreateStudentEvaluationRequestDto](#createstudentevaluationrequestdto)<br>**application/*+json**: [CreateStudentEvaluationRequestDto](#createstudentevaluationrequestdto)<br> | **application/json**: [CreateStudentEvaluationRequestDto](#createstudentevaluationrequestdto)<br>**text/json**: [CreateStudentEvaluationRequestDto](#createstudentevaluationrequestdto)<br>**application/*+json**: [CreateStudentEvaluationRequestDto](#createstudentevaluationrequestdto)<br> | **application/json**: [CreateStudentEvaluationRequestDto](#createstudentevaluationrequestdto)<br>**text/json**: [CreateStudentEvaluationRequestDto](#createstudentevaluationrequestdto)<br>**application/*+json**: [CreateStudentEvaluationRequestDto](#createstudentevaluationrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Evaluation/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/Evaluation/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateStudentEvaluationRequestDto](#updatestudentevaluationrequestdto)<br>**text/json**: [UpdateStudentEvaluationRequestDto](#updatestudentevaluationrequestdto)<br>**application/*+json**: [UpdateStudentEvaluationRequestDto](#updatestudentevaluationrequestdto)<br> | **application/json**: [UpdateStudentEvaluationRequestDto](#updatestudentevaluationrequestdto)<br>**text/json**: [UpdateStudentEvaluationRequestDto](#updatestudentevaluationrequestdto)<br>**application/*+json**: [UpdateStudentEvaluationRequestDto](#updatestudentevaluationrequestdto)<br> | **application/json**: [UpdateStudentEvaluationRequestDto](#updatestudentevaluationrequestdto)<br>**text/json**: [UpdateStudentEvaluationRequestDto](#updatestudentevaluationrequestdto)<br>**application/*+json**: [UpdateStudentEvaluationRequestDto](#updatestudentevaluationrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [DELETE] /api/Evaluation/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Evaluation/student/{studentId}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| studentId | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Evaluation/class/{classId}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| classId | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/Exam
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateExamRequestDto](#createexamrequestdto)<br>**text/json**: [CreateExamRequestDto](#createexamrequestdto)<br>**application/*+json**: [CreateExamRequestDto](#createexamrequestdto)<br> | **application/json**: [CreateExamRequestDto](#createexamrequestdto)<br>**text/json**: [CreateExamRequestDto](#createexamrequestdto)<br>**application/*+json**: [CreateExamRequestDto](#createexamrequestdto)<br> | **application/json**: [CreateExamRequestDto](#createexamrequestdto)<br>**text/json**: [CreateExamRequestDto](#createexamrequestdto)<br>**application/*+json**: [CreateExamRequestDto](#createexamrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Exam
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Exam/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/Exam/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateExamRequestDto](#updateexamrequestdto)<br>**text/json**: [UpdateExamRequestDto](#updateexamrequestdto)<br>**application/*+json**: [UpdateExamRequestDto](#updateexamrequestdto)<br> | **application/json**: [UpdateExamRequestDto](#updateexamrequestdto)<br>**text/json**: [UpdateExamRequestDto](#updateexamrequestdto)<br>**application/*+json**: [UpdateExamRequestDto](#updateexamrequestdto)<br> | **application/json**: [UpdateExamRequestDto](#updateexamrequestdto)<br>**text/json**: [UpdateExamRequestDto](#updateexamrequestdto)<br>**application/*+json**: [UpdateExamRequestDto](#updateexamrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [DELETE] /api/Exam/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Exam/{id}/results
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/Exam/{id}/results/{resultId}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |
| resultId | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateExamResultRequestDto](#updateexamresultrequestdto)<br>**text/json**: [UpdateExamResultRequestDto](#updateexamresultrequestdto)<br>**application/*+json**: [UpdateExamResultRequestDto](#updateexamresultrequestdto)<br> | **application/json**: [UpdateExamResultRequestDto](#updateexamresultrequestdto)<br>**text/json**: [UpdateExamResultRequestDto](#updateexamresultrequestdto)<br>**application/*+json**: [UpdateExamResultRequestDto](#updateexamresultrequestdto)<br> | **application/json**: [UpdateExamResultRequestDto](#updateexamresultrequestdto)<br>**text/json**: [UpdateExamResultRequestDto](#updateexamresultrequestdto)<br>**application/*+json**: [UpdateExamResultRequestDto](#updateexamresultrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/ExamBatch
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateExamBatchRequestDto](#createexambatchrequestdto)<br>**text/json**: [CreateExamBatchRequestDto](#createexambatchrequestdto)<br>**application/*+json**: [CreateExamBatchRequestDto](#createexambatchrequestdto)<br> | **application/json**: [CreateExamBatchRequestDto](#createexambatchrequestdto)<br>**text/json**: [CreateExamBatchRequestDto](#createexambatchrequestdto)<br>**application/*+json**: [CreateExamBatchRequestDto](#createexambatchrequestdto)<br> | **application/json**: [CreateExamBatchRequestDto](#createexambatchrequestdto)<br>**text/json**: [CreateExamBatchRequestDto](#createexambatchrequestdto)<br>**application/*+json**: [CreateExamBatchRequestDto](#createexambatchrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/ExamBatch
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/ExamBatch/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/ExamBatch/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateExamBatchRequestDto](#updateexambatchrequestdto)<br>**text/json**: [UpdateExamBatchRequestDto](#updateexambatchrequestdto)<br>**application/*+json**: [UpdateExamBatchRequestDto](#updateexambatchrequestdto)<br> | **application/json**: [UpdateExamBatchRequestDto](#updateexambatchrequestdto)<br>**text/json**: [UpdateExamBatchRequestDto](#updateexambatchrequestdto)<br>**application/*+json**: [UpdateExamBatchRequestDto](#updateexambatchrequestdto)<br> | **application/json**: [UpdateExamBatchRequestDto](#updateexambatchrequestdto)<br>**text/json**: [UpdateExamBatchRequestDto](#updateexambatchrequestdto)<br>**application/*+json**: [UpdateExamBatchRequestDto](#updateexambatchrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [DELETE] /api/ExamBatch/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PATCH] /api/ExamBatch/{id}/status
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateExamBatchStatusRequestDto](#updateexambatchstatusrequestdto)<br>**text/json**: [UpdateExamBatchStatusRequestDto](#updateexambatchstatusrequestdto)<br>**application/*+json**: [UpdateExamBatchStatusRequestDto](#updateexambatchstatusrequestdto)<br> | **application/json**: [UpdateExamBatchStatusRequestDto](#updateexambatchstatusrequestdto)<br>**text/json**: [UpdateExamBatchStatusRequestDto](#updateexambatchstatusrequestdto)<br>**application/*+json**: [UpdateExamBatchStatusRequestDto](#updateexambatchstatusrequestdto)<br> | **application/json**: [UpdateExamBatchStatusRequestDto](#updateexambatchstatusrequestdto)<br>**text/json**: [UpdateExamBatchStatusRequestDto](#updateexambatchstatusrequestdto)<br>**application/*+json**: [UpdateExamBatchStatusRequestDto](#updateexambatchstatusrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/ExamRegistration
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateExamRegistrationRequestDto](#createexamregistrationrequestdto)<br>**text/json**: [CreateExamRegistrationRequestDto](#createexamregistrationrequestdto)<br>**application/*+json**: [CreateExamRegistrationRequestDto](#createexamregistrationrequestdto)<br> | **application/json**: [CreateExamRegistrationRequestDto](#createexamregistrationrequestdto)<br>**text/json**: [CreateExamRegistrationRequestDto](#createexamregistrationrequestdto)<br>**application/*+json**: [CreateExamRegistrationRequestDto](#createexamregistrationrequestdto)<br> | **application/json**: [CreateExamRegistrationRequestDto](#createexamregistrationrequestdto)<br>**text/json**: [CreateExamRegistrationRequestDto](#createexamregistrationrequestdto)<br>**application/*+json**: [CreateExamRegistrationRequestDto](#createexamregistrationrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PATCH] /api/ExamRegistration/{id}/status
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateExamRegistrationStatusRequestDto](#updateexamregistrationstatusrequestdto)<br>**text/json**: [UpdateExamRegistrationStatusRequestDto](#updateexamregistrationstatusrequestdto)<br>**application/*+json**: [UpdateExamRegistrationStatusRequestDto](#updateexamregistrationstatusrequestdto)<br> | **application/json**: [UpdateExamRegistrationStatusRequestDto](#updateexamregistrationstatusrequestdto)<br>**text/json**: [UpdateExamRegistrationStatusRequestDto](#updateexamregistrationstatusrequestdto)<br>**application/*+json**: [UpdateExamRegistrationStatusRequestDto](#updateexamregistrationstatusrequestdto)<br> | **application/json**: [UpdateExamRegistrationStatusRequestDto](#updateexamregistrationstatusrequestdto)<br>**text/json**: [UpdateExamRegistrationStatusRequestDto](#updateexamregistrationstatusrequestdto)<br>**application/*+json**: [UpdateExamRegistrationStatusRequestDto](#updateexamregistrationstatusrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PATCH] /api/ExamRegistration/{id}/pay
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/ExamRegistration/Batch/{examBatchId}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| examBatchId | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/ExamRegistration/Student/{studentId}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| studentId | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/LearningRoadmap
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateLearningRoadmapRequestDto](#createlearningroadmaprequestdto)<br>**text/json**: [CreateLearningRoadmapRequestDto](#createlearningroadmaprequestdto)<br>**application/*+json**: [CreateLearningRoadmapRequestDto](#createlearningroadmaprequestdto)<br> | **application/json**: [CreateLearningRoadmapRequestDto](#createlearningroadmaprequestdto)<br>**text/json**: [CreateLearningRoadmapRequestDto](#createlearningroadmaprequestdto)<br>**application/*+json**: [CreateLearningRoadmapRequestDto](#createlearningroadmaprequestdto)<br> | **application/json**: [CreateLearningRoadmapRequestDto](#createlearningroadmaprequestdto)<br>**text/json**: [CreateLearningRoadmapRequestDto](#createlearningroadmaprequestdto)<br>**application/*+json**: [CreateLearningRoadmapRequestDto](#createlearningroadmaprequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/LearningRoadmap/course/{courseId}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| courseId | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/LearningRoadmap/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/LearningRoadmap/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateLearningRoadmapRequestDto](#updatelearningroadmaprequestdto)<br>**text/json**: [UpdateLearningRoadmapRequestDto](#updatelearningroadmaprequestdto)<br>**application/*+json**: [UpdateLearningRoadmapRequestDto](#updatelearningroadmaprequestdto)<br> | **application/json**: [UpdateLearningRoadmapRequestDto](#updatelearningroadmaprequestdto)<br>**text/json**: [UpdateLearningRoadmapRequestDto](#updatelearningroadmaprequestdto)<br>**application/*+json**: [UpdateLearningRoadmapRequestDto](#updatelearningroadmaprequestdto)<br> | **application/json**: [UpdateLearningRoadmapRequestDto](#updatelearningroadmaprequestdto)<br>**text/json**: [UpdateLearningRoadmapRequestDto](#updatelearningroadmaprequestdto)<br>**application/*+json**: [UpdateLearningRoadmapRequestDto](#updatelearningroadmaprequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [DELETE] /api/LearningRoadmap/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/Notification
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [SendNotificationRequestDto](#sendnotificationrequestdto)<br>**text/json**: [SendNotificationRequestDto](#sendnotificationrequestdto)<br>**application/*+json**: [SendNotificationRequestDto](#sendnotificationrequestdto)<br> | **application/json**: [SendNotificationRequestDto](#sendnotificationrequestdto)<br>**text/json**: [SendNotificationRequestDto](#sendnotificationrequestdto)<br>**application/*+json**: [SendNotificationRequestDto](#sendnotificationrequestdto)<br> | **application/json**: [SendNotificationRequestDto](#sendnotificationrequestdto)<br>**text/json**: [SendNotificationRequestDto](#sendnotificationrequestdto)<br>**application/*+json**: [SendNotificationRequestDto](#sendnotificationrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Notification/me
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/Notification/{id}/read
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/Question
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateQuestionRequestDto](#createquestionrequestdto)<br>**text/json**: [CreateQuestionRequestDto](#createquestionrequestdto)<br>**application/*+json**: [CreateQuestionRequestDto](#createquestionrequestdto)<br> | **application/json**: [CreateQuestionRequestDto](#createquestionrequestdto)<br>**text/json**: [CreateQuestionRequestDto](#createquestionrequestdto)<br>**application/*+json**: [CreateQuestionRequestDto](#createquestionrequestdto)<br> | **application/json**: [CreateQuestionRequestDto](#createquestionrequestdto)<br>**text/json**: [CreateQuestionRequestDto](#createquestionrequestdto)<br>**application/*+json**: [CreateQuestionRequestDto](#createquestionrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Question
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Question/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | integer |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/Question/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | integer |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateQuestionRequestDto](#updatequestionrequestdto)<br>**text/json**: [UpdateQuestionRequestDto](#updatequestionrequestdto)<br>**application/*+json**: [UpdateQuestionRequestDto](#updatequestionrequestdto)<br> | **application/json**: [UpdateQuestionRequestDto](#updatequestionrequestdto)<br>**text/json**: [UpdateQuestionRequestDto](#updatequestionrequestdto)<br>**application/*+json**: [UpdateQuestionRequestDto](#updatequestionrequestdto)<br> | **application/json**: [UpdateQuestionRequestDto](#updatequestionrequestdto)<br>**text/json**: [UpdateQuestionRequestDto](#updatequestionrequestdto)<br>**application/*+json**: [UpdateQuestionRequestDto](#updatequestionrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [DELETE] /api/Question/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | integer |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/ResourceLearning
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateResourceLearningRequestDto](#createresourcelearningrequestdto)<br>**text/json**: [CreateResourceLearningRequestDto](#createresourcelearningrequestdto)<br>**application/*+json**: [CreateResourceLearningRequestDto](#createresourcelearningrequestdto)<br> | **application/json**: [CreateResourceLearningRequestDto](#createresourcelearningrequestdto)<br>**text/json**: [CreateResourceLearningRequestDto](#createresourcelearningrequestdto)<br>**application/*+json**: [CreateResourceLearningRequestDto](#createresourcelearningrequestdto)<br> | **application/json**: [CreateResourceLearningRequestDto](#createresourcelearningrequestdto)<br>**text/json**: [CreateResourceLearningRequestDto](#createresourcelearningrequestdto)<br>**application/*+json**: [CreateResourceLearningRequestDto](#createresourcelearningrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/ResourceLearning/course/{courseId}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| courseId | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/ResourceLearning/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/ResourceLearning/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateResourceLearningRequestDto](#updateresourcelearningrequestdto)<br>**text/json**: [UpdateResourceLearningRequestDto](#updateresourcelearningrequestdto)<br>**application/*+json**: [UpdateResourceLearningRequestDto](#updateresourcelearningrequestdto)<br> | **application/json**: [UpdateResourceLearningRequestDto](#updateresourcelearningrequestdto)<br>**text/json**: [UpdateResourceLearningRequestDto](#updateresourcelearningrequestdto)<br>**application/*+json**: [UpdateResourceLearningRequestDto](#updateresourcelearningrequestdto)<br> | **application/json**: [UpdateResourceLearningRequestDto](#updateresourcelearningrequestdto)<br>**text/json**: [UpdateResourceLearningRequestDto](#updateresourcelearningrequestdto)<br>**application/*+json**: [UpdateResourceLearningRequestDto](#updateresourcelearningrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [DELETE] /api/ResourceLearning/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/SampleExam
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateSampleExamRequestDto](#createsampleexamrequestdto)<br>**text/json**: [CreateSampleExamRequestDto](#createsampleexamrequestdto)<br>**application/*+json**: [CreateSampleExamRequestDto](#createsampleexamrequestdto)<br> | **application/json**: [CreateSampleExamRequestDto](#createsampleexamrequestdto)<br>**text/json**: [CreateSampleExamRequestDto](#createsampleexamrequestdto)<br>**application/*+json**: [CreateSampleExamRequestDto](#createsampleexamrequestdto)<br> | **application/json**: [CreateSampleExamRequestDto](#createsampleexamrequestdto)<br>**text/json**: [CreateSampleExamRequestDto](#createsampleexamrequestdto)<br>**application/*+json**: [CreateSampleExamRequestDto](#createsampleexamrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/SampleExam
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/SampleExam/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [DELETE] /api/SampleExam/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/SampleExam/{id}/questions
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateSampleExamQuestionsRequestDto](#updatesampleexamquestionsrequestdto)<br>**text/json**: [UpdateSampleExamQuestionsRequestDto](#updatesampleexamquestionsrequestdto)<br>**application/*+json**: [UpdateSampleExamQuestionsRequestDto](#updatesampleexamquestionsrequestdto)<br> | **application/json**: [UpdateSampleExamQuestionsRequestDto](#updatesampleexamquestionsrequestdto)<br>**text/json**: [UpdateSampleExamQuestionsRequestDto](#updatesampleexamquestionsrequestdto)<br>**application/*+json**: [UpdateSampleExamQuestionsRequestDto](#updatesampleexamquestionsrequestdto)<br> | **application/json**: [UpdateSampleExamQuestionsRequestDto](#updatesampleexamquestionsrequestdto)<br>**text/json**: [UpdateSampleExamQuestionsRequestDto](#updatesampleexamquestionsrequestdto)<br>**application/*+json**: [UpdateSampleExamQuestionsRequestDto](#updatesampleexamquestionsrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [POST] /api/SampleExam/{id}/submit
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [SubmitSampleTestRequestDto](#submitsampletestrequestdto)<br>**text/json**: [SubmitSampleTestRequestDto](#submitsampletestrequestdto)<br>**application/*+json**: [SubmitSampleTestRequestDto](#submitsampletestrequestdto)<br> | **application/json**: [SubmitSampleTestRequestDto](#submitsampletestrequestdto)<br>**text/json**: [SubmitSampleTestRequestDto](#submitsampletestrequestdto)<br>**application/*+json**: [SubmitSampleTestRequestDto](#submitsampletestrequestdto)<br> | **application/json**: [SubmitSampleTestRequestDto](#submitsampletestrequestdto)<br>**text/json**: [SubmitSampleTestRequestDto](#submitsampletestrequestdto)<br>**application/*+json**: [SubmitSampleTestRequestDto](#submitsampletestrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/SampleExam/results
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/Schedule
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateClassScheduleRequestDto](#createclassschedulerequestdto)<br>**text/json**: [CreateClassScheduleRequestDto](#createclassschedulerequestdto)<br>**application/*+json**: [CreateClassScheduleRequestDto](#createclassschedulerequestdto)<br> | **application/json**: [CreateClassScheduleRequestDto](#createclassschedulerequestdto)<br>**text/json**: [CreateClassScheduleRequestDto](#createclassschedulerequestdto)<br>**application/*+json**: [CreateClassScheduleRequestDto](#createclassschedulerequestdto)<br> | **application/json**: [CreateClassScheduleRequestDto](#createclassschedulerequestdto)<br>**text/json**: [CreateClassScheduleRequestDto](#createclassschedulerequestdto)<br>**application/*+json**: [CreateClassScheduleRequestDto](#createclassschedulerequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/Schedule/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateClassScheduleRequestDto](#updateclassschedulerequestdto)<br>**text/json**: [UpdateClassScheduleRequestDto](#updateclassschedulerequestdto)<br>**application/*+json**: [UpdateClassScheduleRequestDto](#updateclassschedulerequestdto)<br> | **application/json**: [UpdateClassScheduleRequestDto](#updateclassschedulerequestdto)<br>**text/json**: [UpdateClassScheduleRequestDto](#updateclassschedulerequestdto)<br>**application/*+json**: [UpdateClassScheduleRequestDto](#updateclassschedulerequestdto)<br> | **application/json**: [UpdateClassScheduleRequestDto](#updateclassschedulerequestdto)<br>**text/json**: [UpdateClassScheduleRequestDto](#updateclassschedulerequestdto)<br>**application/*+json**: [UpdateClassScheduleRequestDto](#updateclassschedulerequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [DELETE] /api/Schedule/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Schedule/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Schedule/Class/{classId}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| classId | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PATCH] /api/Schedule/{id}/location
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [AssignLocationRequestDto](#assignlocationrequestdto)<br>**text/json**: [AssignLocationRequestDto](#assignlocationrequestdto)<br>**application/*+json**: [AssignLocationRequestDto](#assignlocationrequestdto)<br> | **application/json**: [AssignLocationRequestDto](#assignlocationrequestdto)<br>**text/json**: [AssignLocationRequestDto](#assignlocationrequestdto)<br>**application/*+json**: [AssignLocationRequestDto](#assignlocationrequestdto)<br> | **application/json**: [AssignLocationRequestDto](#assignlocationrequestdto)<br>**text/json**: [AssignLocationRequestDto](#assignlocationrequestdto)<br>**application/*+json**: [AssignLocationRequestDto](#assignlocationrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [POST] /api/Term
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateTermRequestDto](#createtermrequestdto)<br>**text/json**: [CreateTermRequestDto](#createtermrequestdto)<br>**application/*+json**: [CreateTermRequestDto](#createtermrequestdto)<br> | **application/json**: [CreateTermRequestDto](#createtermrequestdto)<br>**text/json**: [CreateTermRequestDto](#createtermrequestdto)<br>**application/*+json**: [CreateTermRequestDto](#createtermrequestdto)<br> | **application/json**: [CreateTermRequestDto](#createtermrequestdto)<br>**text/json**: [CreateTermRequestDto](#createtermrequestdto)<br>**application/*+json**: [CreateTermRequestDto](#createtermrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Term
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/Term/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateTermRequestDto](#updatetermrequestdto)<br>**text/json**: [UpdateTermRequestDto](#updatetermrequestdto)<br>**application/*+json**: [UpdateTermRequestDto](#updatetermrequestdto)<br> | **application/json**: [UpdateTermRequestDto](#updatetermrequestdto)<br>**text/json**: [UpdateTermRequestDto](#updatetermrequestdto)<br>**application/*+json**: [UpdateTermRequestDto](#updatetermrequestdto)<br> | **application/json**: [UpdateTermRequestDto](#updatetermrequestdto)<br>**text/json**: [UpdateTermRequestDto](#updatetermrequestdto)<br>**application/*+json**: [UpdateTermRequestDto](#updatetermrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [DELETE] /api/Term/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/Term/{id}
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [GET] /api/users/me
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/users/me
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateProfileRequestDto](#updateprofilerequestdto)<br>**text/json**: [UpdateProfileRequestDto](#updateprofilerequestdto)<br>**application/*+json**: [UpdateProfileRequestDto](#updateprofilerequestdto)<br> | **application/json**: [UpdateProfileRequestDto](#updateprofilerequestdto)<br>**text/json**: [UpdateProfileRequestDto](#updateprofilerequestdto)<br>**application/*+json**: [UpdateProfileRequestDto](#updateprofilerequestdto)<br> | **application/json**: [UpdateProfileRequestDto](#updateprofilerequestdto)<br>**text/json**: [UpdateProfileRequestDto](#updateprofilerequestdto)<br>**application/*+json**: [UpdateProfileRequestDto](#updateprofilerequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [DELETE] /api/users/me
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [POST] /api/users/me/apply-staff
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [ApplyStaffRequestDto](#applystaffrequestdto)<br>**text/json**: [ApplyStaffRequestDto](#applystaffrequestdto)<br>**application/*+json**: [ApplyStaffRequestDto](#applystaffrequestdto)<br> | **application/json**: [ApplyStaffRequestDto](#applystaffrequestdto)<br>**text/json**: [ApplyStaffRequestDto](#applystaffrequestdto)<br>**application/*+json**: [ApplyStaffRequestDto](#applystaffrequestdto)<br> | **application/json**: [ApplyStaffRequestDto](#applystaffrequestdto)<br>**text/json**: [ApplyStaffRequestDto](#applystaffrequestdto)<br>**application/*+json**: [ApplyStaffRequestDto](#applystaffrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [POST] /api/users
#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [CreateUserRequestDto](#createuserrequestdto)<br>**text/json**: [CreateUserRequestDto](#createuserrequestdto)<br>**application/*+json**: [CreateUserRequestDto](#createuserrequestdto)<br> | **application/json**: [CreateUserRequestDto](#createuserrequestdto)<br>**text/json**: [CreateUserRequestDto](#createuserrequestdto)<br>**application/*+json**: [CreateUserRequestDto](#createuserrequestdto)<br> | **application/json**: [CreateUserRequestDto](#createuserrequestdto)<br>**text/json**: [CreateUserRequestDto](#createuserrequestdto)<br>**application/*+json**: [CreateUserRequestDto](#createuserrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/users
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/users/students
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [GET] /api/users/instructors
#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [POST] /api/users/{id}/roles/student
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/users/{id}/roles
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Request Body

| Required | Schema |
| -------- | ------ |
|  No | **application/json**: [UpdateUserRolesRequestDto](#updateuserrolesrequestdto)<br>**text/json**: [UpdateUserRolesRequestDto](#updateuserrolesrequestdto)<br>**application/*+json**: [UpdateUserRolesRequestDto](#updateuserrolesrequestdto)<br> | **application/json**: [UpdateUserRolesRequestDto](#updateuserrolesrequestdto)<br>**text/json**: [UpdateUserRolesRequestDto](#updateuserrolesrequestdto)<br>**application/*+json**: [UpdateUserRolesRequestDto](#updateuserrolesrequestdto)<br> | **application/json**: [UpdateUserRolesRequestDto](#updateuserrolesrequestdto)<br>**text/json**: [UpdateUserRolesRequestDto](#updateuserrolesrequestdto)<br>**application/*+json**: [UpdateUserRolesRequestDto](#updateuserrolesrequestdto)<br> |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

### [PUT] /api/users/{id}/toggle-status
#### Parameters

| Name | Located in | Description | Required | Schema |
| ---- | ---------- | ----------- | -------- | ------ |
| id | path |  | Yes | string (uuid) |

#### Responses

| Code | Description |
| ---- | ----------- |
| 200 | OK |

---

### [GET] /WeatherForecast
#### Responses

| Code | Description | Schema |
| ---- | ----------- | ------ |
| 200 | OK | **text/plain**: [ [WeatherForecast](#weatherforecast) ]<br>**application/json**: [ [WeatherForecast](#weatherforecast) ]<br>**text/json**: [ [WeatherForecast](#weatherforecast) ]<br> |

---
### Schemas

#### AnswerOption

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| AnswerOption | integer |  |  |

#### ApplyStaffRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| fullName | string |  | No |
| phone | string |  | Yes |
| dateOfBirth | dateTime |  | No |
| roleId | integer |  | Yes |

#### AssignLocationRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| location | string |  | Yes |

#### AssignStudentsRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| studentIds | [ string (uuid) ] |  | Yes |

#### AssignTeachersRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| instructorIds | [ string (uuid) ] |  | Yes |

#### AssignUsersRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| userIds | [ string (uuid) ] |  | Yes |

#### CancelRegistrationRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| reason | string |  | Yes |

#### ClassStatus

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| ClassStatus | integer |  |  |

#### CourseRegistrationStatus

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| CourseRegistrationStatus | integer |  |  |

#### CreateCenterRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| centerName | string |  | Yes |
| address | string |  | Yes |
| phone | string |  | Yes |
| email | string (email) |  | Yes |

#### CreateClassRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| termId | string (uuid) |  | Yes |
| className | string |  | Yes |
| maxStudents | integer |  | Yes |

#### CreateClassScheduleRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| classId | string (uuid) |  | Yes |
| instructorId | string (uuid) |  | Yes |
| startTime | dateTime |  | Yes |
| endTime | dateTime |  | Yes |
| location | string |  | Yes |

#### CreateCourseRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| centerId | string (uuid) |  | Yes |
| courseName | string |  | Yes |
| licenseType | [ExamLevel](#examlevel) |  | Yes |
| durationInWeeks | integer |  | Yes |
| maxStudents | integer |  | Yes |
| description | string |  | Yes |
| price | double |  | Yes |
| thumbnailUrl | string |  | No |

#### CreateDocumentRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| resourceType | integer |  | No |
| fileUrl | string |  | No |
| fileName | string |  | No |
| extension | string |  | No |
| size | integer |  | No |

#### CreateExamBatchRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| courseId | string (uuid) |  | Yes |
| batchName | string |  | Yes |
| registrationStartDate | dateTime |  | Yes |
| registrationEndDate | dateTime |  | Yes |
| examStartDate | dateTime |  | Yes |

#### CreateExamRegistrationRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| examBatchId | string (uuid) |  | Yes |
| studentId | string (uuid) |  | Yes |
| isPaid | boolean |  | No |

#### CreateExamRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| examBatchId | string (uuid) |  | Yes |
| examName | string |  | Yes |
| examDate | dateTime |  | Yes |
| examType | [ExamType](#examtype) |  | Yes |
| durationMinutes | integer |  | Yes |
| totalScore | integer |  | Yes |
| passScore | integer |  | Yes |

#### CreateLearningRoadmapRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| courseId | string (uuid) |  | Yes |
| title | string |  | Yes |
| description | string |  | Yes |
| orderNo | integer |  | Yes |

#### CreateQuestionRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| content | string |  | Yes |
| answerA | string |  | No |
| answerB | string |  | No |
| answerC | string |  | No |
| answerD | string |  | No |
| correctAnswer | [AnswerOption](#answeroption) |  | Yes |
| imageLink | string |  | No |
| explanation | string |  | No |

#### CreateReferralCodeRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| code | string |  | No |

#### CreateResourceLearningRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| courseId | string (uuid) |  | Yes |
| resourceType | [ResourceType](#resourcetype) |  | Yes |
| title | string |  | Yes |
| resourceUrl | string (uri) |  | Yes |

#### CreateSampleExamRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| courseId | string (uuid) |  | Yes |
| examNo | integer |  | Yes |
| level | [ExamLevel](#examlevel) |  | Yes |
| durationMinutes | integer |  | Yes |
| passingScore | integer |  | Yes |

#### CreateStudentEvaluationRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| studentId | string (uuid) |  | Yes |
| classId | string (uuid) |  | No |
| punctualityScore | integer |  | No |
| skillLevel | integer |  | No |
| note | string |  | No |

#### CreateTermRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| courseId | string (uuid) |  | Yes |
| termName | string |  | Yes |
| startDate | dateTime |  | Yes |
| endDate | dateTime |  | Yes |

#### CreateUserRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| email | string (email) |  | Yes |
| password | string |  | Yes |
| fullName | string |  | Yes |
| phone | string |  | Yes |
| roleIds | [ integer ] |  | No |

#### ExamBatchStatus

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| ExamBatchStatus | integer |  |  |

#### ExamLevel

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| ExamLevel | integer |  |  |

#### ExamRegistrationStatus

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| ExamRegistrationStatus | integer |  |  |

#### ExamStatus

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| ExamStatus | integer |  |  |

#### ExamType

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| ExamType | integer |  |  |

#### LoginRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| email | string (email) |  | Yes |
| password | string |  | Yes |

#### MarkAttendanceRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| classScheduleId | string (uuid) |  | Yes |
| studentId | string (uuid) |  | Yes |
| isPresent | boolean |  | Yes |

#### NotificationType

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| NotificationType | integer |  |  |

#### RegisterCourseRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| courseId | string (uuid) |  | Yes |
| totalFee | double |  | Yes |
| notes | string |  | No |

#### RegisterRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| email | string (email) |  | Yes |
| password | string |  | Yes |
| fullName | string |  | Yes |
| phone | string |  | Yes |

#### ResourceType

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| ResourceType | integer |  |  |

#### SampleExamQuestionDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| questionId | integer |  | No |
| order | integer |  | No |

#### SendNotificationRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| title | string |  | Yes |
| content | string |  | Yes |
| type | [NotificationType](#notificationtype) |  | Yes |
| centerId | string (uuid) |  | No |
| targetRoles | [ [UserRole](#userrole) ] |  | No |

#### SubmitSampleTestRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| durationSeconds | integer |  | Yes |
| answers | object |  | Yes |

#### UpdateCenterRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| centerName | string |  | No |
| address | string |  | No |
| phone | string |  | No |
| email | string (email) |  | No |

#### UpdateClassRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| className | string |  | No |
| maxStudents | integer |  | No |
| status | [ClassStatus](#classstatus) |  | No |

#### UpdateClassScheduleRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| startTime | dateTime |  | Yes |
| endTime | dateTime |  | Yes |
| location | string |  | No |
| instructorId | string (uuid) |  | No |

#### UpdateCourseRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| courseName | string |  | No |
| description | string |  | No |
| thumbnailUrl | string |  | No |
| price | double |  | No |
| maxStudents | integer |  | No |
| durationInWeeks | integer |  | No |

#### UpdateDocumentRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| fileUrl | string |  | No |
| extension | string |  | No |
| size | integer |  | No |

#### UpdateExamBatchRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| batchName | string |  | No |
| registrationStartDate | dateTime |  | No |
| registrationEndDate | dateTime |  | No |
| examStartDate | dateTime |  | No |

#### UpdateExamBatchStatusRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| status | [ExamBatchStatus](#exambatchstatus) |  | Yes |

#### UpdateExamRegistrationStatusRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| status | [ExamRegistrationStatus](#examregistrationstatus) |  | Yes |

#### UpdateExamRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| examName | string |  | No |
| examDate | dateTime |  | No |
| examType | [ExamType](#examtype) |  | No |
| durationMinutes | integer |  | No |
| totalScore | integer |  | No |
| passScore | integer |  | No |
| status | [ExamStatus](#examstatus) |  | No |

#### UpdateExamResultRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| score | double |  | Yes |

#### UpdateLearningRoadmapRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| title | string |  | No |
| description | string |  | No |
| orderNo | integer |  | No |

#### UpdateProfileRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| fullName | string |  | No |
| phone | string |  | No |

#### UpdateQuestionRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| content | string |  | Yes |
| answerA | string |  | No |
| answerB | string |  | No |
| answerC | string |  | No |
| answerD | string |  | No |
| correctAnswer | [AnswerOption](#answeroption) |  | Yes |
| explanation | string |  | No |

#### UpdateRegistrationStatusDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| status | [CourseRegistrationStatus](#courseregistrationstatus) |  | Yes |
| reason | string |  | Yes |

#### UpdateResourceLearningRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| resourceType | [ResourceType](#resourcetype) |  | No |
| title | string |  | No |
| resourceUrl | string (uri) |  | No |

#### UpdateSampleExamQuestionsRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| questions | [ [SampleExamQuestionDto](#sampleexamquestiondto) ] |  | Yes |

#### UpdateStudentEvaluationRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| punctualityScore | integer |  | No |
| skillLevel | integer |  | No |
| note | string |  | No |

#### UpdateTermRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| termName | string |  | No |
| startDate | dateTime |  | No |
| endDate | dateTime |  | No |

#### UpdateUserRolesRequestDto

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| roleIds | [ integer ] |  | Yes |

#### UserRole

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| UserRole | integer |  |  |

#### WeatherForecast

| Name | Type | Description | Required |
| ---- | ---- | ----------- | -------- |
| date | date |  | No |
| temperatureC | integer |  | No |
| temperatureF | integer |  | No |
| summary | string |  | No |
