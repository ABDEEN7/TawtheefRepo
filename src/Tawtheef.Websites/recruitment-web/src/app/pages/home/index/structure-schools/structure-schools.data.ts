import { OrgChartNode } from './structure-schools.model';

/**
 * Static placeholder mirroring structure.md. Replace with an API call
 * returning the same OrgChartNode[] shape once the backend is ready.
 *
 * Plain literal tree, no generator functions — `key` identifies each node
 * (used for childrenByKey caching, data-node-key, and
 * StructureSchools.collapseOtherDeepNodes' ancestor matching, which
 * depends on its hierarchical underscore-prefix format, e.g. '0_0_p' is
 * an ancestor of '0_0_p_0') and must stay as-is.
 *
 * `title`/`description` are translation keys, not literal text — but
 * deliberately *not* named after `key` (e.g. "0_0_p_1_9" told you nothing
 * about which node it was). Each uses a readable slug instead (e.g.
 * "schoolsAdminSocialWorker"), prefixed by branch/role to stay unique
 * where job titles repeat across branches (e.g. "IT Technician" exists
 * under both Schools and Specialized Schools). Real copy lives in
 * public/i18n/pages/schools-structure/{en,ar}.json under
 * "OrganizationalStructure.nodes.<slug>.title" / ".description" — edit
 * those JSON files to change what's shown, not this file. One language
 * for both — ngx-translate's `| translate` pipe resolves the active
 * language reactively, so there's no separate EN/AR tree anymore.
 */
export const SCHOOLS_STRUCTURE: OrgChartNode[] = [
  {
    key: '0',
    data: {
      title: 'OrganizationalStructure.nodes.institutions.title',
      description: 'OrganizationalStructure.nodes.institutions.description',
    },
    children: [
      {
        key: '0_0',
        data: {
          title: 'OrganizationalStructure.nodes.schools.title',
          description: 'OrganizationalStructure.nodes.schools.description',
        },
        children: [
          {
            key: '0_0_p',
            data: {
              title: 'OrganizationalStructure.nodes.schoolsPrincipal.title',
              description: 'OrganizationalStructure.nodes.schoolsPrincipal.description',
            },
            children: [
              {
                key: '0_0_p_0',
                data: {
                  title: 'OrganizationalStructure.nodes.schoolsAcademicDeputy.title',
                  description: 'OrganizationalStructure.nodes.schoolsAcademicDeputy.description',
                },
                children: [
                  {
                    key: '0_0_p_0_0',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAcademicKindergarten.title',
                      description: 'OrganizationalStructure.nodes.schoolsAcademicKindergarten.description',
                    },
                  },
                  {
                    key: '0_0_p_0_1',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAcademicEarlyChildhoodEnglish.title',
                      description: 'OrganizationalStructure.nodes.schoolsAcademicEarlyChildhoodEnglish.description',
                    },
                  },
                  {
                    key: '0_0_p_0_2',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAcademicDrama.title',
                      description: 'OrganizationalStructure.nodes.schoolsAcademicDrama.description',
                    },
                  },
                  {
                    key: '0_0_p_0_3',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAcademicEProjects.title',
                      description: 'OrganizationalStructure.nodes.schoolsAcademicEProjects.description',
                    },
                  },
                  {
                    key: '0_0_p_0_4',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAcademicAdditionalSupport.title',
                      description: 'OrganizationalStructure.nodes.schoolsAcademicAdditionalSupport.description',
                    },
                  },
                  {
                    key: '0_0_p_0_5',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAcademicSocialStudies.title',
                      description: 'OrganizationalStructure.nodes.schoolsAcademicSocialStudies.description',
                    },
                  },
                  {
                    key: '0_0_p_0_6',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAcademicMath.title',
                      description: 'OrganizationalStructure.nodes.schoolsAcademicMath.description',
                    },
                  },
                  {
                    key: '0_0_p_0_7',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAcademicIslamicStudies.title',
                      description: 'OrganizationalStructure.nodes.schoolsAcademicIslamicStudies.description',
                    },
                  },
                  {
                    key: '0_0_p_0_8',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAcademicSpecialEducation.title',
                      description: 'OrganizationalStructure.nodes.schoolsAcademicSpecialEducation.description',
                    },
                  },
                  {
                    key: '0_0_p_0_9',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAcademicScience.title',
                      description: 'OrganizationalStructure.nodes.schoolsAcademicScience.description',
                    },
                  },
                  {
                    key: '0_0_p_0_10',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAcademicOther.title',
                      description: 'OrganizationalStructure.nodes.schoolsAcademicOther.description',
                    },
                  },
                ],
              },
              {
                key: '0_0_p_1',
                data: {
                  title: 'OrganizationalStructure.nodes.schoolsAdminDeputy.title',
                  description: 'OrganizationalStructure.nodes.schoolsAdminDeputy.description',
                },
                children: [
                  {
                    key: '0_0_p_1_0',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAdminSupervisorAssistant.title',
                      description: 'OrganizationalStructure.nodes.schoolsAdminSupervisorAssistant.description',
                    },
                  },
                  {
                    key: '0_0_p_1_1',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAdminActivitiesSpecialist.title',
                      description: 'OrganizationalStructure.nodes.schoolsAdminActivitiesSpecialist.description',
                    },
                  },
                  {
                    key: '0_0_p_1_2',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAdminSecretaryAssistant.title',
                      description: 'OrganizationalStructure.nodes.schoolsAdminSecretaryAssistant.description',
                    },
                  },
                  {
                    key: '0_0_p_1_3',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAdminPsychologist.title',
                      description: 'OrganizationalStructure.nodes.schoolsAdminPsychologist.description',
                    },
                  },
                  {
                    key: '0_0_p_1_4',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAdminItTechnician.title',
                      description: 'OrganizationalStructure.nodes.schoolsAdminItTechnician.description',
                    },
                  },
                  {
                    key: '0_0_p_1_5',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAdminSpecialEducationAssistant.title',
                      description: 'OrganizationalStructure.nodes.schoolsAdminSpecialEducationAssistant.description',
                    },
                  },
                  {
                    key: '0_0_p_1_6',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAdminSpeechTherapist.title',
                      description: 'OrganizationalStructure.nodes.schoolsAdminSpeechTherapist.description',
                    },
                  },
                  {
                    key: '0_0_p_1_7',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAdminSupportStaff.title',
                      description: 'OrganizationalStructure.nodes.schoolsAdminSupportStaff.description',
                    },
                  },
                  {
                    key: '0_0_p_1_8',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAdminCareerGuidance.title',
                      description: 'OrganizationalStructure.nodes.schoolsAdminCareerGuidance.description',
                    },
                  },
                  {
                    key: '0_0_p_1_9',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAdminSocialWorker.title',
                      description: 'OrganizationalStructure.nodes.schoolsAdminSocialWorker.description',
                    },
                  },
                  {
                    key: '0_0_p_1_10',
                    data: {
                      title: 'OrganizationalStructure.nodes.schoolsAdminOther.title',
                      description: 'OrganizationalStructure.nodes.schoolsAdminOther.description',
                    },
                  },
                ],
              },
            ],
          },
        ],
      },
      {
        key: '0_1',
        data: {
          title: 'OrganizationalStructure.nodes.kindergartens.title',
          description: 'OrganizationalStructure.nodes.kindergartens.description',
        },
        children: [
          {
            key: '0_1_p',
            data: {
              title: 'OrganizationalStructure.nodes.kindergartensPrincipal.title',
              description: 'OrganizationalStructure.nodes.kindergartensPrincipal.description',
            },
            children: [
              {
                key: '0_1_p_0',
                data: {
                  title: 'OrganizationalStructure.nodes.kindergartensAcademicTitles.title',
                  description: 'OrganizationalStructure.nodes.kindergartensAcademicTitles.description',
                },
                children: [
                  {
                    key: '0_1_p_0_0',
                    data: {
                      title: 'OrganizationalStructure.nodes.kindergartensAcademicTeacher.title',
                      description: 'OrganizationalStructure.nodes.kindergartensAcademicTeacher.description',
                    },
                  },
                  {
                    key: '0_1_p_0_1',
                    data: {
                      title: 'OrganizationalStructure.nodes.kindergartensEnglishTeacher.title',
                      description: 'OrganizationalStructure.nodes.kindergartensEnglishTeacher.description',
                    },
                  },
                ],
              },
              {
                key: '0_1_p_1',
                data: {
                  title: 'OrganizationalStructure.nodes.kindergartensAdminTitles.title',
                  description: 'OrganizationalStructure.nodes.kindergartensAdminTitles.description',
                },
                children: [
                  {
                    key: '0_1_p_1_0',
                    data: {
                      title: 'OrganizationalStructure.nodes.kindergartensAdminSpecialEducationAssistant.title',
                      description: 'OrganizationalStructure.nodes.kindergartensAdminSpecialEducationAssistant.description',
                    },
                  },
                  {
                    key: '0_1_p_1_1',
                    data: {
                      title: 'OrganizationalStructure.nodes.kindergartensAdminSupportStaff.title',
                      description: 'OrganizationalStructure.nodes.kindergartensAdminSupportStaff.description',
                    },
                  },
                  {
                    key: '0_1_p_1_2',
                    data: {
                      title: 'OrganizationalStructure.nodes.kindergartensAdminSocialWorker.title',
                      description: 'OrganizationalStructure.nodes.kindergartensAdminSocialWorker.description',
                    },
                  },
                  {
                    key: '0_1_p_1_3',
                    data: {
                      title: 'OrganizationalStructure.nodes.kindergartensAdminStudentSupervisor.title',
                      description: 'OrganizationalStructure.nodes.kindergartensAdminStudentSupervisor.description',
                    },
                  },
                  {
                    key: '0_1_p_1_4',
                    data: {
                      title: 'OrganizationalStructure.nodes.kindergartensAdminServicesWorker.title',
                      description: 'OrganizationalStructure.nodes.kindergartensAdminServicesWorker.description',
                    },
                  },
                  {
                    key: '0_1_p_1_5',
                    data: {
                      title: 'OrganizationalStructure.nodes.kindergartensAdminOther.title',
                      description: 'OrganizationalStructure.nodes.kindergartensAdminOther.description',
                    },
                  },
                ],
              },
            ],
          },
        ],
      },
      {
        key: '0_2',
        data: {
          title: 'OrganizationalStructure.nodes.specializedSchools.title',
          description: 'OrganizationalStructure.nodes.specializedSchools.description',
        },
        children: [
          {
            key: '0_2_p',
            data: {
              title: 'OrganizationalStructure.nodes.specializedSchoolsPrincipal.title',
              description: 'OrganizationalStructure.nodes.specializedSchoolsPrincipal.description',
            },
            children: [
              {
                key: '0_2_p_0',
                data: {
                  title: 'OrganizationalStructure.nodes.specializedTechnicalDeputy.title',
                  description: 'OrganizationalStructure.nodes.specializedTechnicalDeputy.description',
                },
                children: [
                  {
                    key: '0_2_p_0_0',
                    data: {
                      title: 'OrganizationalStructure.nodes.specializedTechnicalTeacher.title',
                      description: 'OrganizationalStructure.nodes.specializedTechnicalTeacher.description',
                    },
                  },
                ],
              },
              {
                key: '0_2_p_1',
                data: {
                  title: 'OrganizationalStructure.nodes.specializedAcademicDeputy.title',
                  description: 'OrganizationalStructure.nodes.specializedAcademicDeputy.description',
                },
                children: [
                  {
                    key: '0_2_p_1_0',
                    data: {
                      title: 'OrganizationalStructure.nodes.specializedAcademicLifeSkills.title',
                      description: 'OrganizationalStructure.nodes.specializedAcademicLifeSkills.description',
                    },
                  },
                  {
                    key: '0_2_p_1_1',
                    data: {
                      title: 'OrganizationalStructure.nodes.specializedAcademicEProjects.title',
                      description: 'OrganizationalStructure.nodes.specializedAcademicEProjects.description',
                    },
                  },
                  {
                    key: '0_2_p_1_2',
                    data: {
                      title: 'OrganizationalStructure.nodes.specializedAcademicSocialStudies.title',
                      description: 'OrganizationalStructure.nodes.specializedAcademicSocialStudies.description',
                    },
                  },
                  {
                    key: '0_2_p_1_3',
                    data: {
                      title: 'OrganizationalStructure.nodes.specializedAcademicPhysics.title',
                      description: 'OrganizationalStructure.nodes.specializedAcademicPhysics.description',
                    },
                  },
                  {
                    key: '0_2_p_1_4',
                    data: {
                      title: 'OrganizationalStructure.nodes.specializedAcademicChemistry.title',
                      description: 'OrganizationalStructure.nodes.specializedAcademicChemistry.description',
                    },
                  },
                  {
                    key: '0_2_p_1_5',
                    data: {
                      title: 'OrganizationalStructure.nodes.specializedAcademicOtherTeachers.title',
                      description: 'OrganizationalStructure.nodes.specializedAcademicOtherTeachers.description',
                    },
                  },
                ],
              },
              {
                key: '0_2_p_2',
                data: {
                  title: 'OrganizationalStructure.nodes.specializedAdminDeputy.title',
                  description: 'OrganizationalStructure.nodes.specializedAdminDeputy.description',
                },
                children: [
                  {
                    key: '0_2_p_2_0',
                    data: {
                      title: 'OrganizationalStructure.nodes.specializedAdminChemistryLabTech.title',
                      description: 'OrganizationalStructure.nodes.specializedAdminChemistryLabTech.description',
                    },
                  },
                  {
                    key: '0_2_p_2_1',
                    data: {
                      title: 'OrganizationalStructure.nodes.specializedAdminPhysicsLabPrep.title',
                      description: 'OrganizationalStructure.nodes.specializedAdminPhysicsLabPrep.description',
                    },
                  },
                  {
                    key: '0_2_p_2_2',
                    data: {
                      title: 'OrganizationalStructure.nodes.specializedAdminPsychologist.title',
                      description: 'OrganizationalStructure.nodes.specializedAdminPsychologist.description',
                    },
                  },
                  {
                    key: '0_2_p_2_3',
                    data: {
                      title: 'OrganizationalStructure.nodes.specializedAdminItTechnician.title',
                      description: 'OrganizationalStructure.nodes.specializedAdminItTechnician.description',
                    },
                  },
                  {
                    key: '0_2_p_2_4',
                    data: {
                      title: 'OrganizationalStructure.nodes.specializedAdminSocialWorker.title',
                      description: 'OrganizationalStructure.nodes.specializedAdminSocialWorker.description',
                    },
                  },
                  {
                    key: '0_2_p_2_5',
                    data: {
                      title: 'OrganizationalStructure.nodes.specializedAdminStudentAffairsCoordinator.title',
                      description: 'OrganizationalStructure.nodes.specializedAdminStudentAffairsCoordinator.description',
                    },
                  },
                ],
              },
            ],
          },
        ],
      },
    ],
  },
];
