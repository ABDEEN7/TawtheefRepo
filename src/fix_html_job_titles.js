const fs = require('fs');
const path = require('path');

const targetFiles = [
    'ChangeJobStatusApprovedNotification/ChangeJobStatusApprovedNotification.html.cshtml',
    'ChangeJobStatusApprovedNotification/ChangeJobStatusApprovedNotification.ar.html.cshtml',
    'JobCreatedNotification/JobCreatedNotification.ar.html.cshtml',
    'JobDeletedNotification/JobDeletedNotification.html.cshtml',
    'JobUpdatedNotification/JobUpdatedNotification.html.cshtml',
    'JobCreatedNotification/JobCreatedNotification.html.cshtml',
    'JobDeletedNotification/JobDeletedNotification.ar.html.cshtml',
    'JobUpdatedNotification/JobUpdatedNotification.ar.html.cshtml',
    'JobCandidateInvitationSent/JobCandidateInvitationSent.html.cshtml',
    'JobCandidateInvitationSent/JobCandidateInvitationSent.ar.html.cshtml',
    'ChangeJobStatusNotification/ChangeJobStatusNotification.html.cshtml',
    'ChangeJobStatusRejectedNotification/ChangeJobStatusRejectedNotification.html.cshtml',
    'ChangeJobStatusRejectedNotification/ChangeJobStatusRejectedNotification.ar.html.cshtml',
    'ChangeJobStatusNotification/ChangeJobStatusNotification.ar.html.cshtml',
    'ChangeJobStatusNeedUpdateNotification/ChangeJobStatusNeedUpdateNotification.ar.html.cshtml',
    'ChangeJobStatusNeedUpdateNotification/ChangeJobStatusNeedUpdateNotification.html.cshtml'
];

const baseDir = 'c:/Users/t-a.jaber/RiderProjects/Tawtheef/src/Notifications/Templates';

targetFiles.forEach(relPath => {
    const filePath = path.join(baseDir, relPath);
    if (!fs.existsSync(filePath)) return;

    let content = fs.readFileSync(filePath, 'utf8');

    // 1. Fix the variable assignment
    // var jobTitle = string.IsNullOrWhiteSpace(m.JobTitle) ? "..." : m.JobTitle;
    content = content.replace(/var jobTitle = string\.IsNullOrWhiteSpace\(m\.JobTitle\) \? \".*\" : m\.JobTitle;/g, 'var jobTitle = m.JobTitle;');

    // 2. Fix the usage in HTML
    // <strong>@jobTitle</strong>
    content = content.replace(/<strong>@jobTitle<\/strong>/g, '@(string.IsNullOrWhiteSpace(jobTitle) ? "" : $" <strong>\\"{jobTitle}\\"</strong>")');
    
    // Also handle just @jobTitle if it has quotes around it in the template
    content = content.replace(/\"@jobTitle\"/g, '@(string.IsNullOrWhiteSpace(jobTitle) ? "" : $" \\"{jobTitle}\\"")');

    fs.writeFileSync(filePath, content, 'utf8');
    console.log('Fixed HTML: ' + relPath);
});
