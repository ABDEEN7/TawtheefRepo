/* ---------------------------------------------
   Candidate Types
---------------------------------------------- */
export enum CandidateType {
  Qatari = "Qatari",
  GCC = "GCC",
  ResidentQatar = "ResidentQatar",
  NonQatari = "NonQatari",
  SonOfQatariMother = "SonOfQatariMother",
  WifeOfQatari = "WifeOfQatari",
  QidHolder = "QidHolder",
}

/* ---------------------------------------------
   Invitation / Application Status
---------------------------------------------- */
export enum InvitationStatus {
  NewInvitation = "NewInvitation",
  Closed = "Closed",
  Cancelled = "Cancelled",
  Read = "Read",
  ReturnedAttachment = "ReturnedAttachment",
  PendingAttachmentApproval = "PendingAttachmentApproval",
  Rejected = "Rejected",
  ExamEligible = "ExamEligible",
  Expired = 'Expired',
}

/* ---------------------------------------------
   Degree
---------------------------------------------- */
export enum Degree {
  Doctorate = "Doctorate",
  Master = "Master",
  PostgraduateDiploma = "PostgraduateDiploma",
  Bachelor = "Bachelor",
  IntermediateDiploma = "IntermediateDiploma",
  Secondary = "Secondary",
  Preparatory = "Preparatory",
  Primary = "Primary",
  NoQualifications = "NoQualifications",
}

/* ---------------------------------------------
   Departments
---------------------------------------------- */
export enum Department {
  InformationSystems = "InformationSystems",
  HumanResources = "HumanResources",
  AdministrativeFinancialAffairs = "AdministrativeFinancialAffairs",
  Evaluation = "Evaluation",
  Curriculum = "Curriculum",
  EarlyChildhoodEducation = "EarlyChildhoodEducation",
  HigherEducation = "HigherEducation",
  PrimaryEducation = "PrimaryEducation",
  SchoolAffairs = "SchoolAffairs",
  CommunicationsMedia = "CommunicationsMedia",
}

/* ---------------------------------------------
   Gender
---------------------------------------------- */
export enum Gender {
  All = "All",
  Male = "Male",
  Female = "Female",
}

/* ---------------------------------------------
   Job Category
---------------------------------------------- */
export enum JobCategory {
  Academic = "Academic",
  Administrative = "Administrative",
  Labor = "Labor",
}

/* ---------------------------------------------
   Job Status
---------------------------------------------- */
export enum JobStatus {
  Draft = "Draft",
  NeedUpdate = "NeedUpdate",
  Closed = "Closed",
  Cancelled = "Cancelled",
  PendingApproval = "PendingApproval",
  PendingPointConfiguration = "PendingPointConfiguration",
  NeedPointUpdate = "NeedPointUpdate",
  PendingPointApproval = "PendingPointApproval",
  ReadyForAnnouncement = "ReadyForAnnouncement",
  Published = "Published",
  Rejected = "Rejected"
}

/* ---------------------------------------------
   Profile Status
---------------------------------------------- */
export enum ProfileStatus {
  InCreation = "InCreation",
  Submitted = "Submitted",
  UnderReview = "UnderReview",
  RequiresUpdate = "RequiresUpdate",
  Approved = "Approved",
  Rejected = "Rejected",
  Cancelled = "Cancelled",
  AdminCancelled = "AdminCancelled",
}
export enum ProfileStatusNumber {
  InCreation = 0,
  Submitted = 1,
  UnderReview = 2,
  RequiresUpdate = 3,
  Approved = 4,
  Rejected = 5,
  Cancelled = 6,
  AdminCancelled = 7,
}
/* ---------------------------------------------
   Language
---------------------------------------------- */
export enum Language {
  Arabic = "Arabic",
  English = "English",
}

/* ---------------------------------------------
   Language Level
---------------------------------------------- */
export enum LanguageLevel {
  Basic = "Basic",
  Intermediate = "Intermediate",
  Advanced = "Advanced",
  Native = "Native",
}

/* ---------------------------------------------
   Marital Status
---------------------------------------------- */
export enum MaritalStatus {
  Single = "Single",
  Married = "Married",
  Divorced = "Divorced",
  Widowed = "Widowed",
}
/* ---------------------------------------------
   Rating Grade
---------------------------------------------- */
export enum RatingGrade {
  Excellent = "Excellent",
  VeryGood = "VeryGood",
  Good = "Good",
  Acceptable = "Acceptable",
}

/* ---------------------------------------------
   Religion
---------------------------------------------- */
export enum Religion {
  Islam = "Islam",
  Christian = "Christian",
  Hindu = "Hindu",
  Buddhist = "Buddhist",
  Sikh = "Sikh",
  Other = "Other",
}

/* ---------------------------------------------
   Sponsor Type
---------------------------------------------- */
export enum SponsorType {
  Individual = "Individual",
  Company = "Company",
}

/* ---------------------------------------------
   Sector
---------------------------------------------- */
export enum Sector {
  Schools = "Schools",
  Ministry = "Ministry",
}

/* ---------------------------------------------
   Study Type
---------------------------------------------- */
export enum StudyType {
  Regular = "Regular",
  DistanceLearning = "DistanceLearning",
  Affiliation = "Affiliation",
}

/* ---------------------------------------------
   Target Entity
---------------------------------------------- */
export enum TargetEntity {
  Schools = "Schools",
  Ministry = "Ministry",
}
/* ---------------------------------------------
   Work Type
---------------------------------------------- */
export enum WorkType {
  FullTime = "FullTime",
  PartTime = "PartTime",
}
