import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { SharedModule } from '../../../../shared/shared.module';
import { I18nNamespaceDirective } from '../../../../shared/directives/i18n-namespace.directive';

export type StructureNodeType = 'leadership' | 'department' | 'special' | 'section';

export interface StructureNode {
  id: string;
  title: string;
  type: StructureNodeType;
  expanded?: boolean;
  description?: string;
  responsibilities?: string[];
  children?: StructureNode[];
}

@Component({
  selector: 'app-structure-schools',
  standalone: true,
  imports: [CommonModule, SharedModule, I18nNamespaceDirective],
  templateUrl: './structure-schools.html',
  styleUrls: ['./structure-schools.scss'],
})
export class StructureSchools implements AfterViewInit {
  @ViewChild('rootNode')
  rootNode!: ElementRef<HTMLElement>;

  isDragging = false;
  selectedNode: StructureNode | null = null;

  private dragStartX = 0;
  private dragStartY = 0;
  private startingScrollLeft = 0;
  private startingScrollTop = 0;
  private scrollAnimationFrame?: number;

  root: StructureNode = {
    id: 'institutions',
    title: 'OrganizationalStructure.nodes.institutions.title',
    type: 'leadership',
    description: 'OrganizationalStructure.nodes.institutions.description',
    expanded: true,
  };

  categories: StructureNode[] = [
    {
      id: 'schools',
      title: 'OrganizationalStructure.nodes.schools.title',
      type: 'leadership',
      description: 'OrganizationalStructure.nodes.schools.description',
      expanded: false,
      children: [
        {
          id: 'schools-principal',
          title: 'OrganizationalStructure.nodes.schoolsPrincipal.title',
          type: 'special',
          description: 'OrganizationalStructure.nodes.schoolsPrincipal.description',
          expanded: false,
          children: [
            {
              id: 'schools-academic-deputy',
              title: 'OrganizationalStructure.nodes.schoolsAcademicDeputy.title',
              type: 'department',
              description: 'OrganizationalStructure.nodes.schoolsAcademicDeputy.description',
              expanded: false,
              children: [
                {
                  id: 'schools-academic-kindergarten',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicKindergarten.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAcademicKindergarten.description',
                },
                {
                  id: 'schools-academic-early-childhood-english',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicEarlyChildhoodEnglish.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAcademicEarlyChildhoodEnglish.description',
                },
                {
                  id: 'schools-academic-drama',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicDrama.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAcademicDrama.description',
                },
                {
                  id: 'schools-academic-e-projects',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicEProjects.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAcademicEProjects.description',
                },
                {
                  id: 'schools-academic-additional-support',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicAdditionalSupport.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAcademicAdditionalSupport.description',
                },
                {
                  id: 'schools-academic-social-studies',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicSocialStudies.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAcademicSocialStudies.description',
                },
                {
                  id: 'schools-academic-math',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicMath.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAcademicMath.description',
                },
                {
                  id: 'schools-academic-islamic-studies',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicIslamicStudies.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAcademicIslamicStudies.description',
                },
                {
                  id: 'schools-academic-special-education',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicSpecialEducation.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAcademicSpecialEducation.description',
                },
                {
                  id: 'schools-academic-science',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicScience.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAcademicScience.description',
                },
                {
                  id: 'schools-academic-other',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicOther.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAcademicOther.description',
                },
                {
                  id: 'schools-academic-physics',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicPhysics.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAcademicPhysics.description',
                },
                {
                  id: 'schools-academic-chemistry',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicChemistry.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAcademicChemistry.description',
                },
                {
                  id: 'schools-academic-arabic',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicArabic.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAcademicArabic.description',
                },
                {
                  id: 'schools-academic-english',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicEnglish.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAcademicEnglish.description',
                },
                {
                  id: 'schools-academic-pe',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicPhysicalEducation.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAcademicPhysicalEducation.description',
                },
                {
                  id: 'schools-academic-scientific-track',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicScientificTrack.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAcademicScientificTrack.description',
                },
                {
                  id: 'schools-academic-computer',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicComputer.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAcademicComputer.description',
                },
                {
                  id: 'schools-academic-visual-arts',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicVisualArts.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAcademicVisualArts.description',
                },
                {
                  id: 'schools-academic-literary-track',
                  title: 'OrganizationalStructure.nodes.schoolsAcademicLiteraryTrack.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAcademicLiteraryTrack.description',
                },
              ],
            },
            {
              id: 'schools-admin-deputy',
              title: 'OrganizationalStructure.nodes.schoolsAdminDeputy.title',
              type: 'department',
              description: 'OrganizationalStructure.nodes.schoolsAdminDeputy.description',
              expanded: false,
              children: [
                {
                  id: 'schools-admin-supervisor-assistant',
                  title: 'OrganizationalStructure.nodes.schoolsAdminSupervisorAssistant.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAdminSupervisorAssistant.description',
                },
                {
                  id: 'schools-admin-activities-specialist',
                  title: 'OrganizationalStructure.nodes.schoolsAdminActivitiesSpecialist.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAdminActivitiesSpecialist.description',
                },
                {
                  id: 'schools-admin-secretary-assistant',
                  title: 'OrganizationalStructure.nodes.schoolsAdminSecretaryAssistant.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAdminSecretaryAssistant.description',
                },
                {
                  id: 'schools-admin-psychologist',
                  title: 'OrganizationalStructure.nodes.schoolsAdminPsychologist.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAdminPsychologist.description',
                },
                {
                  id: 'schools-admin-it-technician',
                  title: 'OrganizationalStructure.nodes.schoolsAdminItTechnician.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAdminItTechnician.description',
                },
                {
                  id: 'schools-admin-special-education-assistant',
                  title:
                    'OrganizationalStructure.nodes.schoolsAdminSpecialEducationAssistant.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAdminSpecialEducationAssistant.description',
                },
                {
                  id: 'schools-admin-speech-therapist',
                  title: 'OrganizationalStructure.nodes.schoolsAdminSpeechTherapist.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAdminSpeechTherapist.description',
                },
                {
                  id: 'schools-admin-support-staff',
                  title: 'OrganizationalStructure.nodes.schoolsAdminSupportStaff.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAdminSupportStaff.description',
                },
                {
                  id: 'schools-admin-career-guidance',
                  title: 'OrganizationalStructure.nodes.schoolsAdminCareerGuidance.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAdminCareerGuidance.description',
                },
                {
                  id: 'schools-admin-social-worker',
                  title: 'OrganizationalStructure.nodes.schoolsAdminSocialWorker.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAdminSocialWorker.description',
                },
                {
                  id: 'schools-admin-other',
                  title: 'OrganizationalStructure.nodes.schoolsAdminOther.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAdminOther.description',
                },
                {
                  id: 'schools-admin-student-supervisor',
                  title: 'OrganizationalStructure.nodes.schoolsAdminStudentSupervisor.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAdminStudentSupervisor.description',
                },
                {
                  id: 'schools-admin-student-affairs-coordinator',
                  title:
                    'OrganizationalStructure.nodes.schoolsAdminStudentAffairsCoordinator.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAdminStudentAffairsCoordinator.description',
                },
                {
                  id: 'schools-admin-academic-guidance',
                  title: 'OrganizationalStructure.nodes.schoolsAdminAcademicGuidance.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAdminAcademicGuidance.description',
                },
                {
                  id: 'schools-admin-supervisor',
                  title: 'OrganizationalStructure.nodes.schoolsAdminSupervisor.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAdminSupervisor.description',
                },
                {
                  id: 'schools-admin-secretary',
                  title: 'OrganizationalStructure.nodes.schoolsAdminSecretary.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAdminSecretary.description',
                },
                {
                  id: 'schools-admin-storekeeper',
                  title: 'OrganizationalStructure.nodes.schoolsAdminStorekeeper.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAdminStorekeeper.description',
                },
                {
                  id: 'schools-admin-accountant',
                  title: 'OrganizationalStructure.nodes.schoolsAdminAccountant.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAdminAccountant.description',
                },
                {
                  id: 'schools-admin-receptionist',
                  title: 'OrganizationalStructure.nodes.schoolsAdminReceptionist.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAdminReceptionist.description',
                },
                {
                  id: 'schools-admin-cafeteria-supervisor',
                  title: 'OrganizationalStructure.nodes.schoolsAdminCafeteriaSupervisor.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAdminCafeteriaSupervisor.description',
                },
                {
                  id: 'schools-admin-delegate',
                  title: 'OrganizationalStructure.nodes.schoolsAdminDelegate.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.schoolsAdminDelegate.description',
                },
                {
                  id: 'schools-admin-services-worker',
                  title: 'OrganizationalStructure.nodes.schoolsAdminServicesWorker.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.schoolsAdminServicesWorker.description',
                },
              ],
            },
          ],
        },
      ],
    },
    {
      id: 'kindergartens',
      title: 'OrganizationalStructure.nodes.kindergartens.title',
      type: 'leadership',
      description: 'OrganizationalStructure.nodes.kindergartens.description',
      expanded: false,
      children: [
        {
          id: 'kindergartens-principal',
          title: 'OrganizationalStructure.nodes.kindergartensPrincipal.title',
          type: 'special',
          description: 'OrganizationalStructure.nodes.kindergartensPrincipal.description',
          expanded: false,
          children: [
            {
              id: 'kindergartens-academic-titles',
              title: 'OrganizationalStructure.nodes.kindergartensAcademicTitles.title',
              type: 'department',
              description: 'OrganizationalStructure.nodes.kindergartensAcademicTitles.description',
              expanded: false,
              children: [
                {
                  id: 'kindergartens-academic-teacher',
                  title: 'OrganizationalStructure.nodes.kindergartensAcademicTeacher.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.kindergartensAcademicTeacher.description',
                },
                {
                  id: 'kindergartens-english-teacher',
                  title: 'OrganizationalStructure.nodes.kindergartensEnglishTeacher.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.kindergartensEnglishTeacher.description',
                },
              ],
            },
            {
              id: 'kindergartens-admin-titles',
              title: 'OrganizationalStructure.nodes.kindergartensAdminTitles.title',
              type: 'department',
              description: 'OrganizationalStructure.nodes.kindergartensAdminTitles.description',
              expanded: false,
              children: [
                {
                  id: 'kindergartens-admin-special-education-assistant',
                  title:
                    'OrganizationalStructure.nodes.kindergartensAdminSpecialEducationAssistant.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.kindergartensAdminSpecialEducationAssistant.description',
                },
                {
                  id: 'kindergartens-admin-support-staff',
                  title: 'OrganizationalStructure.nodes.kindergartensAdminSupportStaff.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.kindergartensAdminSupportStaff.description',
                },
                {
                  id: 'kindergartens-admin-social-worker',
                  title: 'OrganizationalStructure.nodes.kindergartensAdminSocialWorker.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.kindergartensAdminSocialWorker.description',
                },
                {
                  id: 'kindergartens-admin-student-supervisor',
                  title: 'OrganizationalStructure.nodes.kindergartensAdminStudentSupervisor.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.kindergartensAdminStudentSupervisor.description',
                },
                {
                  id: 'kindergartens-admin-services-worker',
                  title: 'OrganizationalStructure.nodes.kindergartensAdminServicesWorker.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.kindergartensAdminServicesWorker.description',
                },
                {
                  id: 'kindergartens-admin-other',
                  title: 'OrganizationalStructure.nodes.kindergartensAdminOther.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.kindergartensAdminOther.description',
                },
                {
                  id: 'kindergartens-admin-teacher-assistant',
                  title: 'OrganizationalStructure.nodes.kindergartensAdminTeacherAssistant.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.kindergartensAdminTeacherAssistant.description',
                },
                {
                  id: 'kindergartens-admin-supervisor',
                  title: 'OrganizationalStructure.nodes.kindergartensAdminSupervisor.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.kindergartensAdminSupervisor.description',
                },
                {
                  id: 'kindergartens-admin-receptionist',
                  title: 'OrganizationalStructure.nodes.kindergartensAdminReceptionist.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.kindergartensAdminReceptionist.description',
                },
                {
                  id: 'kindergartens-admin-e-projects',
                  title: 'OrganizationalStructure.nodes.kindergartensAdminEProjects.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.kindergartensAdminEProjects.description',
                },
              ],
            },
          ],
        },
      ],
    },
    {
      id: 'specialized-schools',
      title: 'OrganizationalStructure.nodes.specializedSchools.title',
      type: 'leadership',
      description: 'OrganizationalStructure.nodes.specializedSchools.description',
      expanded: false,
      children: [
        {
          id: 'specialized-schools-principal',
          title: 'OrganizationalStructure.nodes.specializedSchoolsPrincipal.title',
          type: 'special',
          description: 'OrganizationalStructure.nodes.specializedSchoolsPrincipal.description',
          expanded: false,
          children: [
            {
              id: 'specialized-technical-deputy',
              title: 'OrganizationalStructure.nodes.specializedTechnicalDeputy.title',
              type: 'department',
              description: 'OrganizationalStructure.nodes.specializedTechnicalDeputy.description',
              expanded: false,
              children: [
                {
                  id: 'specialized-technical-teacher',
                  title: 'OrganizationalStructure.nodes.specializedTechnicalTeacher.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedTechnicalTeacher.description',
                },
              ],
            },
            {
              id: 'specialized-academic-deputy',
              title: 'OrganizationalStructure.nodes.specializedAcademicDeputy.title',
              type: 'department',
              description: 'OrganizationalStructure.nodes.specializedAcademicDeputy.description',
              expanded: false,
              children: [
                {
                  id: 'specialized-academic-life-skills',
                  title: 'OrganizationalStructure.nodes.specializedAcademicLifeSkills.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAcademicLifeSkills.description',
                },
                {
                  id: 'specialized-academic-e-projects',
                  title: 'OrganizationalStructure.nodes.specializedAcademicEProjects.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAcademicEProjects.description',
                },
                {
                  id: 'specialized-academic-social-studies',
                  title: 'OrganizationalStructure.nodes.specializedAcademicSocialStudies.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAcademicSocialStudies.description',
                },
                {
                  id: 'specialized-academic-physics',
                  title: 'OrganizationalStructure.nodes.specializedAcademicPhysics.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAcademicPhysics.description',
                },
                {
                  id: 'specialized-academic-chemistry',
                  title: 'OrganizationalStructure.nodes.specializedAcademicChemistry.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAcademicChemistry.description',
                },
                {
                  id: 'specialized-academic-other-teachers',
                  title: 'OrganizationalStructure.nodes.specializedAcademicOtherTeachers.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAcademicOtherTeachers.description',
                },
                {
                  id: 'specialized-academic-math',
                  title: 'OrganizationalStructure.nodes.specializedAcademicMath.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.specializedAcademicMath.description',
                },
                {
                  id: 'specialized-academic-islamic-studies',
                  title: 'OrganizationalStructure.nodes.specializedAcademicIslamicStudies.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAcademicIslamicStudies.description',
                },
                {
                  id: 'specialized-academic-arabic',
                  title: 'OrganizationalStructure.nodes.specializedAcademicArabic.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAcademicArabic.description',
                },
                {
                  id: 'specialized-academic-english',
                  title: 'OrganizationalStructure.nodes.specializedAcademicEnglish.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAcademicEnglish.description',
                },
                {
                  id: 'specialized-academic-pe',
                  title: 'OrganizationalStructure.nodes.specializedAcademicPhysicalEducation.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAcademicPhysicalEducation.description',
                },
              ],
            },
            {
              id: 'specialized-admin-deputy',
              title: 'OrganizationalStructure.nodes.specializedAdminDeputy.title',
              type: 'department',
              description: 'OrganizationalStructure.nodes.specializedAdminDeputy.description',
              expanded: false,
              children: [
                {
                  id: 'specialized-admin-chemistry-lab-tech',
                  title: 'OrganizationalStructure.nodes.specializedAdminChemistryLabTech.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminChemistryLabTech.description',
                },
                {
                  id: 'specialized-admin-physics-lab-prep',
                  title: 'OrganizationalStructure.nodes.specializedAdminPhysicsLabPrep.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminPhysicsLabPrep.description',
                },
                {
                  id: 'specialized-admin-psychologist',
                  title: 'OrganizationalStructure.nodes.specializedAdminPsychologist.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminPsychologist.description',
                },
                {
                  id: 'specialized-admin-it-technician',
                  title: 'OrganizationalStructure.nodes.specializedAdminItTechnician.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminItTechnician.description',
                },
                {
                  id: 'specialized-admin-social-worker',
                  title: 'OrganizationalStructure.nodes.specializedAdminSocialWorker.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminSocialWorker.description',
                },
                {
                  id: 'specialized-admin-student-affairs-coordinator',
                  title:
                    'OrganizationalStructure.nodes.specializedAdminStudentAffairsCoordinator.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminStudentAffairsCoordinator.description',
                },
                {
                  id: 'specialized-admin-workshop-technician',
                  title: 'OrganizationalStructure.nodes.specializedAdminWorkshopTechnician.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminWorkshopTechnician.description',
                },
                {
                  id: 'specialized-admin-academic-guidance',
                  title: 'OrganizationalStructure.nodes.specializedAdminAcademicGuidance.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminAcademicGuidance.description',
                },
                {
                  id: 'specialized-admin-student-supervisor',
                  title: 'OrganizationalStructure.nodes.specializedAdminStudentSupervisor.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminStudentSupervisor.description',
                },
                {
                  id: 'specialized-admin-delegate',
                  title: 'OrganizationalStructure.nodes.specializedAdminDelegate.title',
                  type: 'section',
                  description: 'OrganizationalStructure.nodes.specializedAdminDelegate.description',
                },
                {
                  id: 'specialized-admin-services-worker',
                  title: 'OrganizationalStructure.nodes.specializedAdminServicesWorker.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminServicesWorker.description',
                },
                {
                  id: 'specialized-admin-cafeteria-supervisor',
                  title: 'OrganizationalStructure.nodes.specializedAdminCafeteriaSupervisor.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminCafeteriaSupervisor.description',
                },
                {
                  id: 'specialized-admin-supervisor',
                  title: 'OrganizationalStructure.nodes.specializedAdminSupervisor.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminSupervisor.description',
                },
                {
                  id: 'specialized-admin-secretary',
                  title: 'OrganizationalStructure.nodes.specializedAdminSecretary.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminSecretary.description',
                },
                {
                  id: 'specialized-admin-storekeeper',
                  title: 'OrganizationalStructure.nodes.specializedAdminStorekeeper.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminStorekeeper.description',
                },
                {
                  id: 'specialized-admin-receptionist',
                  title: 'OrganizationalStructure.nodes.specializedAdminReceptionist.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminReceptionist.description',
                },
                {
                  id: 'specialized-admin-accountant',
                  title: 'OrganizationalStructure.nodes.specializedAdminAccountant.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminAccountant.description',
                },
                {
                  id: 'specialized-admin-special-education-assistant',
                  title:
                    'OrganizationalStructure.nodes.specializedAdminSpecialEducationAssistant.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminSpecialEducationAssistant.description',
                },
                {
                  id: 'specialized-admin-support-staff',
                  title: 'OrganizationalStructure.nodes.specializedAdminSupportStaff.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminSupportStaff.description',
                },
                {
                  id: 'specialized-admin-activities-specialist',
                  title: 'OrganizationalStructure.nodes.specializedAdminActivitiesSpecialist.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminActivitiesSpecialist.description',
                },
                {
                  id: 'specialized-admin-speech-language-technician',
                  title:
                    'OrganizationalStructure.nodes.specializedAdminSpeechLanguageTechnician.title',
                  type: 'section',
                  description:
                    'OrganizationalStructure.nodes.specializedAdminSpeechLanguageTechnician.description',
                },
              ],
            },
          ],
        },
      ],
    },
  ];

  ngAfterViewInit(): void {
    requestAnimationFrame(() => {
      if (!window.matchMedia('(max-width: 767.98px)').matches) {
        this.rootNode.nativeElement.scrollIntoView({
          behavior: 'auto',
          block: 'nearest',
          inline: 'center',
        });
      }
    });
  }

  toggleRoot(event: MouseEvent): void {
    event.stopPropagation();

    const shouldOpen = !this.root.expanded;

    this.root.expanded = shouldOpen;

    if (shouldOpen) {
      this.centerOpenedContent(event.currentTarget as HTMLElement, '.assistant-level');
    } else {
      this.closeNodes(this.categories);
    }
  }

  toggleNode(node: StructureNode, event: MouseEvent): void {
    event.stopPropagation();

    if (!node.children?.length) {
      return;
    }

    const willExpand = !node.expanded;

    if (this.categories.includes(node)) {
      this.closeOtherCategories(node);
      node.expanded = willExpand;
    } else if (willExpand && node.type === 'department') {
      this.closeOtherDepartments(node);
      node.expanded = true;
    } else {
      node.expanded = willExpand;
    }

    if (node.expanded) {
      this.centerOpenedNode(event.currentTarget as HTMLElement);
    } else {
      this.closeNodes(node.children);
    }
  }

  showNodeInformation(node: StructureNode, event: MouseEvent): void {
    event.stopPropagation();
    this.selectedNode = node;
  }

  closeNodeInformation(): void {
    this.selectedNode = null;
  }

  stopModalClick(event: MouseEvent): void {
    event.stopPropagation();
  }

  @HostListener('document:keydown.escape')
  closeModalWithEscape(): void {
    this.closeNodeInformation();
  }

  startDragging(event: PointerEvent): void {
    if (window.matchMedia('(max-width: 767.98px)').matches) {
      return;
    }

    const target = event.target as HTMLElement;

    if (target.closest('button')) {
      return;
    }

    const map = event.currentTarget as HTMLElement;

    if (this.scrollAnimationFrame !== undefined) {
      cancelAnimationFrame(this.scrollAnimationFrame);

      this.scrollAnimationFrame = undefined;
    }

    this.isDragging = true;
    this.dragStartX = event.clientX;
    this.dragStartY = event.clientY;
    this.startingScrollLeft = map.scrollLeft;
    this.startingScrollTop = map.scrollTop;

    map.setPointerCapture(event.pointerId);
  }

  dragMap(event: PointerEvent): void {
    if (!this.isDragging) {
      return;
    }

    const map = event.currentTarget as HTMLElement;

    const movementX = event.clientX - this.dragStartX;

    const movementY = event.clientY - this.dragStartY;

    map.scrollLeft = this.startingScrollLeft - movementX;

    map.scrollTop = this.startingScrollTop - movementY;
  }

  stopDragging(event: PointerEvent): void {
    if (!this.isDragging) {
      return;
    }

    const map = event.currentTarget as HTMLElement;

    this.isDragging = false;

    if (map.hasPointerCapture(event.pointerId)) {
      map.releasePointerCapture(event.pointerId);
    }
  }

  trackByNodeId(index: number, node: StructureNode): string {
    return node.id;
  }

  private closeNodes(nodes: StructureNode[]): void {
    nodes.forEach((node) => {
      node.expanded = false;

      if (node.children?.length) {
        this.closeNodes(node.children);
      }
    });
  }

  private closeOtherCategories(openedNode: StructureNode): void {
    this.categories.forEach((category) => {
      if (category !== openedNode) {
        category.expanded = false;

        if (category.children?.length) {
          this.closeNodes(category.children);
        }
      }
    });
  }

  private closeOtherDepartments(openedNode: StructureNode): void {
    const closeOthers = (nodes: StructureNode[]): void => {
      nodes.forEach((node) => {
        if (node !== openedNode && node.type === 'department') {
          node.expanded = false;

          if (node.children?.length) {
            this.closeNodes(node.children);
          }
        }

        if (node.children?.length) {
          closeOthers(node.children);
        }
      });
    };

    closeOthers(this.categories);
  }

  private centerOpenedContent(trigger: HTMLElement, selector: string): void {
    requestAnimationFrame(() => {
      requestAnimationFrame(() => {
        const tree = trigger.closest('.structure-tree');

        const openedContent = tree?.querySelector<HTMLElement>(selector);

        if (openedContent) {
          this.animateMapToElement(trigger, openedContent);
        }
      });
    });
  }

  private centerOpenedNode(trigger: HTMLElement): void {
    requestAnimationFrame(() => {
      requestAnimationFrame(() => {
        const openedBranch = trigger.closest<HTMLElement>('.assistant-branch, .deputy-branch');

        const openedContent = openedBranch?.querySelector<HTMLElement>(
          '.deputies-row, .education-children',
        );

        const target = openedContent ?? openedBranch;

        if (target) {
          this.animateMapToElement(trigger, target);
        }
      });
    });
  }

  private animateMapToElement(trigger: HTMLElement, target: HTMLElement): void {
    if (window.matchMedia('(max-width: 767.98px)').matches) {
      target.scrollIntoView({
        behavior: 'smooth',
        block: 'center',
        inline: 'nearest',
      });

      return;
    }

    const map = trigger.closest<HTMLElement>('.structure-map');

    if (!map) {
      return;
    }

    if (this.scrollAnimationFrame !== undefined) {
      cancelAnimationFrame(this.scrollAnimationFrame);
    }

    const mapRect = map.getBoundingClientRect();

    const targetRect = target.getBoundingClientRect();

    const startLeft = map.scrollLeft;
    const startTop = map.scrollTop;

    const requestedLeft =
      startLeft + targetRect.left - mapRect.left - (map.clientWidth - targetRect.width) / 2;

    const requestedTop =
      startTop + targetRect.top - mapRect.top - (map.clientHeight - targetRect.height) / 2;

    const targetLeft = requestedLeft;

    const targetTop = Math.max(0, Math.min(requestedTop, map.scrollHeight - map.clientHeight));

    const distanceLeft = targetLeft - startLeft;

    const distanceTop = targetTop - startTop;

    const duration = 950;
    const startTime = performance.now();

    const easeInOutCubic = (progress: number): number =>
      progress < 0.5 ? 4 * progress * progress * progress : 1 - Math.pow(-2 * progress + 2, 3) / 2;

    const animate = (currentTime: number): void => {
      const progress = Math.min((currentTime - startTime) / duration, 1);

      const easedProgress = easeInOutCubic(progress);

      map.scrollLeft = startLeft + distanceLeft * easedProgress;

      map.scrollTop = startTop + distanceTop * easedProgress;

      if (progress < 1) {
        this.scrollAnimationFrame = requestAnimationFrame(animate);
      } else {
        this.scrollAnimationFrame = undefined;
      }
    };

    this.scrollAnimationFrame = requestAnimationFrame(animate);
  }
}
