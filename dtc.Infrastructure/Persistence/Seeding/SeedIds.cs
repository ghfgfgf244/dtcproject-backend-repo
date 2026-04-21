using System;

namespace dtc.Infrastructure.Persistence.Seeding
{
    internal static class SeedIds
    {
        public static readonly Guid UserA = Guid.Parse("11111111-1111-1111-1111-111111111111");
        public static readonly Guid UserB = Guid.Parse("22222222-2222-2222-2222-222222222222");

        public static readonly Guid CenterA = Guid.Parse("33333333-3333-3333-3333-333333333331");
        public static readonly Guid CenterB = Guid.Parse("33333333-3333-3333-3333-333333333332");

        public static readonly Guid CourseA = Guid.Parse("44444444-4444-4444-4444-444444444441");
        public static readonly Guid CourseB = Guid.Parse("44444444-4444-4444-4444-444444444442");

        public static readonly Guid TermA = Guid.Parse("55555555-5555-5555-5555-555555555551");
        public static readonly Guid TermB = Guid.Parse("55555555-5555-5555-5555-555555555552");

        public static readonly Guid ClassA = Guid.Parse("66666666-6666-6666-6666-666666666661");
        public static readonly Guid ClassB = Guid.Parse("66666666-6666-6666-6666-666666666662");

        public static readonly Guid ClassScheduleA = Guid.Parse("77777777-7777-7777-7777-777777777771");
        public static readonly Guid ClassScheduleB = Guid.Parse("77777777-7777-7777-7777-777777777772");

        public static readonly Guid ExamBatchA = Guid.Parse("88888888-8888-8888-8888-888888888881");
        public static readonly Guid ExamBatchB = Guid.Parse("88888888-8888-8888-8888-888888888882");

        public static readonly Guid ExamA = Guid.Parse("99999999-9999-9999-9999-999999999991");
        public static readonly Guid ExamB = Guid.Parse("99999999-9999-9999-9999-999999999992");

        public static readonly Guid ExamRegistrationA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa1");
        public static readonly Guid ExamRegistrationB = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa2");
        public static readonly Guid ExamResultA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa3");
        public static readonly Guid ExamResultB = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaa4");

        public static readonly Guid ReferralCodeA = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb1");
        public static readonly Guid ReferralCodeB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb2");
        public static readonly Guid ReferralRegistrationA = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb3");
        public static readonly Guid ReferralRegistrationB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb4");
        public static readonly Guid CollaboratorCommissionA = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb5");
        public static readonly Guid CollaboratorCommissionB = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbb6");

        public static readonly Guid DocumentA = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc1");
        public static readonly Guid DocumentB = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc2");
        public static readonly Guid StudentDrivingDistanceA = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc3");
        public static readonly Guid StudentDrivingDistanceB = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc4");
        public static readonly Guid InstructorLeaveRequestA = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc5");
        public static readonly Guid InstructorLeaveRequestB = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc6");
        public static readonly Guid StudentEvaluationA = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc7");
        public static readonly Guid StudentEvaluationB = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc8");
        public static readonly Guid AttendanceA = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccc9");
        public static readonly Guid AttendanceB = Guid.Parse("cccccccc-cccc-cccc-cccc-ccccccccccca");

        public static readonly Guid CourseRegistrationA = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd1");
        public static readonly Guid CourseRegistrationB = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd2");
        public static readonly Guid SampleExamResultA = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd3");
        public static readonly Guid SampleExamResultB = Guid.Parse("dddddddd-dddd-dddd-dddd-ddddddddddd4");

        public static readonly Guid CategoryA = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee1");
        public static readonly Guid CategoryB = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee2");
        public static readonly Guid BlogA = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee3");
        public static readonly Guid BlogB = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee4");
        public static readonly Guid CategoryC = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeef");
        public static readonly Guid CategoryD = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeed1");
        public static readonly Guid CategoryE = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeed2");
        public static readonly Guid BlogC = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeef0");
        public static readonly Guid BlogD = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeed3");
        public static readonly Guid BlogE = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeed4");
        public static readonly Guid BlogF = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeed5");
        public static readonly Guid SampleExamA = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee5");
        public static readonly Guid SampleExamB = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee6");
        public static readonly Guid SampleExamQuestionA = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee7");
        public static readonly Guid SampleExamQuestionB = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee8");
        public static readonly Guid LearningLocationA = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeee9");
        public static readonly Guid LearningLocationB = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeea");
        public static readonly Guid LearningLocationC = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeed6");
        public static readonly Guid LearningLocationD = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeed7");
        public static readonly Guid LearningLocationE = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeed8");
        public static readonly Guid NotificationA = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeeb");
        public static readonly Guid NotificationB = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeec");
        public static readonly Guid UserNotificationA = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeed");
        public static readonly Guid UserNotificationB = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        public static readonly Guid LearningRoadmapA = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff1");
        public static readonly Guid LearningRoadmapB = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff2");
        public static readonly Guid LearningRoadmapC = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff5");
        public static readonly Guid LearningRoadmapD = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff6");
        public static readonly Guid LearningRoadmapE = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff7");
        public static readonly Guid LearningRoadmapF = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff8");
        public static readonly Guid ResourceLearningA = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff3");
        public static readonly Guid ResourceLearningB = Guid.Parse("ffffffff-ffff-ffff-ffff-fffffffffff4");
    }
}
