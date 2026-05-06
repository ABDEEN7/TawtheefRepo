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
}

/* ---------------------------------------------
   Invitation / Application Status
---------------------------------------------- */
export enum InvitationStatus {
  NewInvitation = "NewInvitation",
  Closed = "Closed",
  Cancelled = "Cancelled",
  ReturnedAttachment = "ReturnedAttachment",
  ExamEligible = "ExamEligible",
  PendingAttachmentApproval = "PendingAttachmentApproval",
  Read = "Read",
  Rejected = "Rejected",
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
  Published = "Published",
  Closed = "Closed",
  Cancelled = "Cancelled",
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
