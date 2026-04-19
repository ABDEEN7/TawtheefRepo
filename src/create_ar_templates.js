const fs = require('fs');
const path = require('path');

const baseDir = 'c:/Users/t-a.jaber/RiderProjects/Tawtheef/src/Notifications/Templates';
const dirs = fs.readdirSync(baseDir, { withFileTypes: true }).filter(d => d.isDirectory());

for (const d of dirs) {
    const dirPath = path.join(baseDir, d.name);
    const htmlEnPath = path.join(dirPath, d.name + '.html.cshtml');
    const htmlArPath = path.join(dirPath, d.name + '.ar.html.cshtml');
    
    // Create AR HTML if missing
    if (fs.existsSync(htmlEnPath) && !fs.existsSync(htmlArPath)) {
        let content = fs.readFileSync(htmlEnPath, 'utf8');
        content = content.replace(/lang=\"en\"/, 'lang=\"ar\" dir=\"rtl\"');
        
        // Very basic replacements for common headers
        content = content.replace(/font-family: Arial, sans-serif;/g, "font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;");
        
        // Specific mapping based on d.name
        if (d.name === 'ChangeJobStatusApprovedNotification') {
            content = content.replace('Your job posting has been approved', 'تمت الموافقة على طلب الوظيفة');
            content = content.replace('Dear Colleague,', 'عزيزي الزميل/الزميلة،');
            content = content.replace(/The job posting <strong>(.*?)<\/strong> has been approved./, 'تمت الموافقة على طلب الوظيفة <strong>$1</strong>.');
            content = content.replace('You may proceed with publishing the role or taking the next steps in', 'يمكنك الآن المتابعة في نشر الوظيفة أو اتخاذ الخطوات التالية في');
            content = content.replace('>View in', '>عرض في');
            content = content.replace('If the button does not work, copy and paste this link into your browser:', 'إذا لم يعمل الزر، انسخ هذا الرابط والصقه في المتصفح:');
            content = content.replace('Regards,<br>', 'مع تحيات،<br>فريق عمل ');
            content = content.replace('Support:', 'للدعم الفني:');
            content = content.replace('All rights reserved.', 'جميع الحقوق محفوظة.');
            content = content.replace('Approved', 'تمت الموافقة');
            content = content.replace('Job Posting Approved', 'تمت الموافقة على الوظيفة');
        }
        else if (d.name === 'ChangeJobStatusNeedUpdateNotification') {
            content = content.replace('Updates are required for your job posting', 'طلب الوظيفة بحاجة لتحديثات');
            content = content.replace('Updates required', 'مطلوب تحديث');
            content = content.replace('Dear Colleague,', 'عزيزي الزميل/الزميلة،');
            content = content.replace(/The job posting <strong>(.*?)<\/strong> requires updates before it can be approved./, 'يتطلب طلب الوظيفة <strong>$1</strong> تحديثات إضافية قبل المتابعة.');
            content = content.replace('Please sign in to @(product), review the requested changes, and resubmit the posting.', 'يرجى مراجعة الملاحظات على المنصة وإجراء التعديلات المطلوبة.');
            content = content.replace('Please sign in to @product, review the requested changes, and resubmit the posting.', 'يرجى مراجعة الملاحظات على المنصة وإجراء التعديلات المطلوبة.');
            content = content.replace('>Update in', '>تحديث في');
            content = content.replace('If the button does not work, copy and paste this link into your browser:', 'إذا لم يعمل الزر، انسخ هذا الرابط والصقه في المتصفح:');
            content = content.replace('Regards,<br>', 'مع تحيات،<br>فريق عمل ');
            content = content.replace('Support:', 'للدعم الفني:');
            content = content.replace('All rights reserved.', 'جميع الحقوق محفوظة.');
        }
        else if (d.name === 'ChangeJobStatusNotification') {
            content = content.replace('Job status change requires review', 'تغيير حالة الوظيفة بانتظار المراجعة');
            content = content.replace('Review required', 'بانتظار المراجعة');
            content = content.replace('Dear Colleague,', 'عزيزي الزميل/الزميلة،');
            content = content.replace(/A request has been submitted to change the status of <strong>(.*?)<\/strong>./, 'تم تقديم طلب لتغيير حالة <strong>$1</strong>.');
            content = content.replace('Please sign in to @product to review the request and proceed with the appropriate action.', 'يرجى تسجيل الدخول إلى المنصة لمراجعة الطلب والمتابعة.');
            content = content.replace('>Review request in', '>مراجعة الطلب في');
            content = content.replace('If the button does not work, copy and paste this link into your browser:', 'إذا لم يعمل الزر، انسخ هذا الرابط والصقه في المتصفح:');
            content = content.replace('Regards,<br>', 'مع تحيات،<br>فريق عمل ');
            content = content.replace('Support:', 'للدعم الفني:');
            content = content.replace('All rights reserved.', 'جميع الحقوق محفوظة.');
        }
        else if (d.name === 'ChangeJobStatusRejectedNotification') {
            content = content.replace('Your job posting was not approved', 'لم تتم الموافقة على طلب الوظيفة');
            content = content.replace('Rejected', 'تم الرفض');
            content = content.replace('Dear Colleague,', 'عزيزي الزميل/الزميلة،');
            content = content.replace(/The request for <strong>(.*?)<\/strong> has been rejected./, 'تم رفض طلب الوظيفة <strong>$1</strong>.');
            content = content.replace('Please review the feedback in @product, update the posting as needed, and resubmit.', 'يرجى مراجعة المنصة للاطلاع على الأسباب أو الملاحظات.');
            content = content.replace('>Review feedback in', '>مراجعة الملاحظات في');
            content = content.replace('If the button does not work, copy and paste this link into your browser:', 'إذا لم يعمل الزر، انسخ هذا الرابط والصقه في المتصفح:');
            content = content.replace('Regards,<br>', 'مع تحيات،<br>فريق عمل ');
            content = content.replace('Support:', 'للدعم الفني:');
            content = content.replace('All rights reserved.', 'جميع الحقوق محفوظة.');
        }
        else if (d.name === 'FinalizeReviewProfile') {
            content = content.replace('Profile Approved', 'تمت الموافقة على الملف الشخصي');
            content = content.replace('Action Required', 'مطلوب إجراء');
            content = content.replace('Dear Candidate,', 'مرحباً،');
            content = content.replace('We are pleased to inform you that your profile has been approved on @product.', 'يسرنا إبلاغكم بأنه تمت الموافقة على ملفكم الشخصي في منصة @product.');
            content = content.replace('You can now continue using the platform and access all available features.', 'يمكنكم الآن الاستمرار في استخدام المنصة والوصول إلى كافة الميزات المتاحة.');
            content = content.replace('>View Profile<', '>عرض الملف الشخصي<');
            content = content.replace('Your profile has been returned for requested updates.', 'تمت إعادة ملفكم الشخصي لإجراء بعض التحديثات المطلوبة.');
            content = content.replace('Please review the notes below, make the necessary corrections, and resubmit.', 'يرجى مراجعة الملاحظات أدناه، وإجراء التصحيحات اللازمة، ثم إعادة تقديم الملف.');
            content = content.replace('Reviewer Notes:', 'ملاحظات المراجع:');
            content = content.replace('>Update Profile<', '>تحديث الملف الشخصي<');
            content = content.replace('Regards,<br>', 'مع أطيب التحيات،<br>');
            content = content.replace('If you have any questions, we are happy to assist you at:', 'إذا كان لديكم أي استفسارات، يسعدنا تواصلكم معنا عبر:');
            content = content.replace('If you need assistance, we are happy to help at:', 'إذا كنتم بحاجة للمساعدة، يسعدنا تواصلكم عبر:');
            content = content.replace('All rights reserved.', 'جميع الحقوق محفوظة.');
            content = content.replace('Support:', 'دعم:');
        }
        else if (d.name === 'InvitationAttachmentReturned') {
            content = content.replace('Action Required: Attachment Returned', 'مطلوب إجراء: إعادة المرفق');
            content = content.replace('Dear Candidate,', 'عزيزي المرشح،');
            content = content.replace('One of your submitted attachments for your job application requires your attention.', 'تتطلب إحدى المرفقات المقدمة لطلب الوظيفة مراجعتك.');
            content = content.replace('Attachment:', 'المرفق:');
            content = content.replace('Review Note:', 'ملاحظة المراجع:');
            content = content.replace('Please sign in to the recruitment portal to re-upload the corrected document and re-submit your application.', 'يرجى تسجيل الدخول إلى المنصة لإعادة تحميل المستند المصحح وإعادة تقديم طلبك.');
            content = content.replace('>Sign in to portal<', '>تسجيل الدخول للمنصة<');
            content = content.replace('>Sign in<', '>تسجيل الدخول<');
            content = content.replace('Regards,<br>', 'مع تحيات،<br>فريق عمل ');
            content = content.replace('Support:', 'للدعم الفني:');
            content = content.replace('All rights reserved.', 'جميع الحقوق محفوظة.');
        }
        else if (d.name === 'ProfileAssigned') {
            content = content.replace('New Profile Assigned for Review', 'تم تعيين ملف جديد للمراجعة');
            content = content.replace('Hello,', 'مرحباً،');
            content = content.replace('You have been assigned a new profile for review in @product.', 'تم تعيين ملف مرشح جديد تحت مراجعتك في @product.');
            content = content.replace('Candidate Name:', 'اسم المرشح:');
            content = content.replace('Please visit the following link to review the profile:', 'يرجى زيارة الرابط التالي لمراجعة الملف:');
            content = content.replace('>Review Profile<', '>مراجعة الملف<');
            content = content.replace('Regards,<br>', 'مع تحيات،<br>فريق عمل ');
            content = content.replace('Support:', 'للدعم الفني:');
            content = content.replace('All rights reserved.', 'جميع الحقوق محفوظة.');
        }
        
        fs.writeFileSync(htmlArPath, '\uFEFF' + content, 'utf8');
        console.log('Created ' + d.name + '.ar.html.cshtml');
    }
    
    // Missing ChangeJobStatusNotification.ar.txt.cshtml specially
    if (d.name === 'ChangeJobStatusNotification') {
        const txtArPath = path.join(dirPath, d.name + '.ar.txt.cshtml');
        if (!fs.existsSync(txtArPath)) {
            const txtAr = `@model Tawtheef.Notifications.Context.TemplateContext<Tawtheef.Notifications.Templates.ChangeJobStatusNotification.ChangeJobStatusNotificationModel>

العنوان: تغيير حالة الوظيفة بانتظار المراجعة

عزيزي الزميل/الزميلة،

تم تقديم طلب لتغيير حالة "@(string.IsNullOrWhiteSpace(Model.Model.JobTitle) ? "الوظيفة" : Model.Model.JobTitle)".

يرجى تسجيل الدخول إلى @Model.Branding.ProductName لمراجعة الطلب والمتابعة.

رابط المنصة: @Model.Branding.WebsiteUrl

مع تحيات،
فريق عمل @Model.Branding.ProductName
للدعم الفني: @Model.Branding.SupportEmail
`;
            fs.writeFileSync(txtArPath, '\uFEFF' + txtAr, 'utf8');
            console.log('Created ChangeJobStatusNotification.ar.txt.cshtml');
        }
    }
}
