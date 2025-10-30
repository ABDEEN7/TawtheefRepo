import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';


import { UserProfileWizard } from './user-profile-wizard/user-profile-wizard';
import { PersonalInfo } from './steps/personal-info/personal-info';
import { ContactInfo } from './steps/contact-info/contact-info';
import { Education } from './steps/education/education';
import { Experience } from './steps/experience/experience';
import { Skills } from './steps/skills/skills';
import { Attachments } from './steps/attachments/attachments';
import { Review } from './steps/review/review';
import { TraningCourseModal } from './modals/traning-course-modal/traning-course-modal';
import { DegreeModal } from './modals/degree-modal/degree-modal';
import { ExperienceModal } from './modals/experience-modal/experience-modal';
import { UserProfileRoutingModule} from './user-profile-routing.module';


@NgModule({
  declarations: [
    UserProfileWizard,
    PersonalInfo,
    ContactInfo,
    Education,
    Experience,
    Skills,
    Attachments,
    Review,
    TraningCourseModal,
    DegreeModal,
    ExperienceModal,

  ],
  imports: [CommonModule, FormsModule, ReactiveFormsModule, UserProfileRoutingModule],
  exports: [UserProfileWizard]
})
export class ProfileModule {}
