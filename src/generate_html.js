const fs = require('fs');

const data = JSON.parse(fs.readFileSync('c:/Users/t-a.jaber/RiderProjects/Tawtheef/src/templates_data.json', 'utf8').replace(/^\uFEFF/, ''));

const hardcoded = {};

const mapTemplateToReason = {
  'QatarResidentOtp': { reason: 'تسجيل الدخول للمقيم (Qatar Resident OTP)', loc: 'RequestQatarResidentOtpCommandHandler.cs', channel: 'SMS' },
  'PhoneVerificationCode': { reason: 'تأكيد رقم الهاتف (Phone Verification OTP)', loc: 'RequestPhoneVerificationCommandHandler.cs', channel: 'SMS' },
  'MinisterOfficeRegistration': { reason: 'تسجيل مرشح جديد لمكتب الوزير', loc: 'CreateMinisterOfficeCandidateCommandHandler.cs', channel: 'SMS' },
  'MinisterOfficeNewCandidateHr': { reason: 'إشعار الموارد البشرية بمرشح جديد لمكتب الوزير', loc: 'CreateMinisterOfficeCandidateCommandHandler.cs', channel: 'Email, InApp' },
  'KawaderInvitation': { reason: 'دعوة مستخدمي كوادر', loc: 'UploadKawaderUserCommandHandler.cs', channel: 'Email, SMS' },
  'FinalizeReviewProfile': { reason: 'الانتهاء من مراجعة الملف الشخصي (قبول أو طلب تحديث)', loc: 'FinalizeReviewProfileEventHandler.cs', channel: 'Email, InApp' },
  'ProfileAssigned': { reason: 'تعيين ملف شخصي لموظف', loc: 'ProfileAssignedEventHandler.cs', channel: 'Email, InApp' },
  'OfficeCreatedNotification': { reason: 'إنشاء مكتب جديد', loc: 'OfficeCreatedDomainEventHandler.cs', channel: 'Email' },
  'ChangeJobStatusApprovedNotification': { reason: 'الموافقة على تغيير حالة الوظيفة', loc: 'ChangeJobStatusApprovedNotificationDomainEventHandler.cs', channel: 'Email' },
  'ChangeJobStatusNeedUpdateNotification': { reason: 'طلب تحديث حالة الوظيفة', loc: 'ChangeJobStatusNeedUpdateNotificationDomainEventHandler.cs', channel: 'Email' },
  'ChangeJobStatusNotification': { reason: 'إشعار بتغيير حالة الوظيفة', loc: 'ChangeJobStatusNotificationDomainEventHandler.cs', channel: 'Email' },
  'ChangeJobStatusRejectedNotification': { reason: 'رفض تغيير حالة الوظيفة', loc: 'ChangeJobStatusRejectedNotificationDomainEventHandler.cs', channel: 'Email' },
  'JobCreatedNotification': { reason: 'إنشاء وظيفة جديدة', loc: 'JobCreatedDomainEventHandler.cs', channel: 'Email' },
  'JobDeletedNotification': { reason: 'حذف وظيفة', loc: 'JobDeletedDomainEventHandler.cs', channel: 'Email' },
  'JobUpdatedNotification': { reason: 'تحديث بيانات وظيفة', loc: 'JobUpdatedDomainEventHandler.cs', channel: 'Email' },
  'JobCandidateInvitationSent': { reason: 'إرسال دعوة لمرشح للوظيفة', loc: 'JobCandidateInvitationSentDomainEventHandler.cs', channel: 'Email, InApp, SMS' },
  'InvitationAttachmentReturned': { reason: 'إرجاع مرفق الدعوة للمرشح', loc: 'InvitationAttachmentReturnedEventHandler.cs', channel: 'Email, InApp, SMS' },
  'ContactVerificationSent': { reason: 'إرسال رمز التحقق (لجهات الاتصال عبر البريد والإس إم إس)', loc: 'ContactVerificationSentEventHandler.cs', channel: 'Email, SMS' }
};

function cleanTemplate(txt) {
  if (!txt) return '';
  let lines = txt.split('\n');
  let start = 0;
  for (let i = 0; i < lines.length; i++) {
    if (lines[i].includes('@model Tawtheef.Notifications')) {
      start = i + 1;
      break;
    }
  }
  let filtered = lines.slice(start);
  let res = filtered.filter(l => !l.trim().startsWith('@{') && !l.trim().startsWith('var ') && !l.trim().startsWith('}') && !l.trim().startsWith('@*') && !l.trim().startsWith('*@') && !l.trim().includes('using Tawtheef')).join('\n');
  return res.replace(/@:\s*/g, '').replace(/@:/g, '').trim();
}

let html = `<!DOCTYPE html>
<html lang="ar" dir="rtl">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>ملخص إشعارات النظام (Notification.Create)</title>
  <style>
    body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7f6; color: #333; margin: 0; padding: 20px; }
    h1 { text-align: center; color: #2c3e50; }
    .card { background: #fff; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); margin-bottom: 20px; padding: 20px; }
    .card-header { border-bottom: 2px solid #3498db; padding-bottom: 10px; margin-bottom: 15px; }
    .card-header h2 { margin: 0; color: #2980b9; font-size: 20px; }
    .meta { display: flex; flex-wrap: wrap; gap: 15px; margin-bottom: 15px; font-size: 14px; background: #ecf0f1; padding: 10px; border-radius: 5px; }
    .meta div { flex: 1; min-width: 250px; }
    .meta strong { color: #2c3e50; }
    .content-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
    .lang-section { background: #fdfdfd; border: 1px solid #e0e0e0; border-radius: 5px; padding: 15px; }
    .lang-title { font-weight: bold; margin-bottom: 10px; color: #16a085; text-align: center; border-bottom: 1px solid #eee; padding-bottom: 5px; }
    pre { background: #f8f9fa; padding: 10px; border-radius: 5px; border: 1px solid #ccc; white-space: pre-wrap; word-wrap: break-word; font-family: inherit; font-size: 14px; line-height: 1.5; color: #444; margin: 0;}
    .subject { font-weight: bold; margin-bottom: 10px; display: block; color: #e74c3c; }
  </style>
</head>
<body>
  <h1>ملخص استخدامات إشعارات النظام (Notification.Create)</h1>`;

let counter = 1;
for (let key in data) {
  let mapped = mapTemplateToReason[key] || { reason: key, loc: 'غير محدد', channel: 'غير محدد' };
  
  let arTxt = cleanTemplate(data[key][key + '.ar.txt.cshtml']);
  let enTxt = cleanTemplate(data[key][key + '.txt.cshtml']);
  
  // Custom Overrides for Edge Cases
  if (key === 'FinalizeReviewProfile') {
      arTxt = "حالة الموافقة:\nالعنوان: تمت الموافقة على ملفكم الشخصي - @product\nمرحباً،\nيسرنا إبلاغكم بأنه تمت الموافقة على ملفكم الشخصي في منصة @product.\nيمكنكم الآن الاستمرار في استخدام المنصة والوصول إلى كافة الميزات المتاحة.\nرابط الملف الشخصي:\n@profileUrl\nإذا كان لديكم أي استفسارات، يسعدنا تواصلكم معنا عبر:\n@support\nمع أطيب التحيات،\nفريق عمل @product\n\nحالة طلب تحديث:\nالعنوان: مطلوب إجراء – ملفكم الشخصي بحاجة لتحديثات - @product\nمرحباً،\nتمت إعادة ملفكم الشخصي لإجراء بعض التحديثات المطلوبة.\nيرجى مراجعة الملاحظات أدناه، وإجراء التصحيحات اللازمة، ثم إعادة تقديم الملف.\nملاحظات المراجع: @notes\nيرجى تحديث ملفكم عبر الرابط التالي: @profileUrl\nإذا كنتم بحاجة للمساعدة، يسعدنا تواصلكم عبر:\n@support\nمع أطيب التحيات،\nفريق عمل @product";
      enTxt = "Approval Case:\nSubject: Profile Approved - @product\nDear Candidate,\nWe are pleased to inform you that your profile has been approved on @product.\nYou can now continue using the platform and access all available features.\nProfile Link:\n@profileUrl\nIf you have any questions, we are happy to assist you at:\n@support\nBest regards,\n@product Team\n\nUpdate Required Case:\nSubject: Action Required – Profile Needs Updates - @product\nDear Candidate,\nYour profile has been returned for requested updates.\nPlease review the notes below, make the necessary corrections, and resubmit.\nReviewer Notes: @notes\nPlease update your profile using the following link: @profileUrl\nIf you need assistance, we are happy to help at:\n@support\nBest regards,\n@product Team";
  }

  // Try to find Subject
  let arSubject = '';
  let arBody = arTxt;
  let arLines = arTxt.split('\n');
  if (key !== 'FinalizeReviewProfile') {
      if(arLines[0] && (arLines[0].includes('العنوان:') || arLines[0].includes('Subject:'))) {
         arSubject = arLines[0].replace('العنوان:', '').replace('Subject:', '').trim();
         arBody = arLines.slice(1).join('\n').trim();
      }
      // If it doesn't contain Explicit "العنوان:", assume the whole thing is the body. 
  }

  // Try to find Subject in EN
  let enSubject = '';
  let enBody = enTxt;
  let enLines = enTxt.split('\n');
  if (key !== 'FinalizeReviewProfile') {
      if(enLines[0] && (enLines[0].includes('العنوان:') || enLines[0].includes('Subject:'))) {
         enSubject = enLines[0].replace('Subject:', '').replace('العنوان:', '').trim();
         enBody = enLines.slice(1).join('\n').trim();
      }
  }
  
  if (!arTxt && !arSubject) {
      if(key === 'ChangeJobStatusNotification') {
          arBody = "لا يوجد قالب باللغة العربية معرّف حالياً (The .ar template is missing).";
      } else {
          arBody = "غير متوفر.";
      }
  }

  html += `
  <div class="card">
    <div class="card-header">
      <h2>${counter}. ${mapped.reason}</h2>
    </div>
    <div class="meta">
      <div><strong>مكان الاستخدام:</strong> ${mapped.loc}</div>
      <div><strong>قناة الإرسال:</strong> ${mapped.channel}</div>
    </div>
    <div class="content-grid">
      <div class="lang-section">
        <div class="lang-title">الرسالة باللغة العربية</div>
        ${arSubject ? '<span class="subject">العنوان: ' + arSubject + '</span>' : ''}
        <pre dir="rtl">${arBody}</pre>
      </div>
      <div class="lang-section" dir="ltr">
        <div class="lang-title">الرسالة باللغة الإنجليزية</div>
        ${enSubject ? '<span class="subject">Subject: ' + enSubject + '</span>' : ''}
        <pre dir="ltr">${enBody}</pre>
      </div>
    </div>
  </div>`;
  counter++;
}

// Add hardcoded
for (let key in hardcoded) {
  let item = hardcoded[key];
  html += `
  <div class="card">
    <div class="card-header">
      <h2>${counter}. ${item.name} (نص ثابت في الكود)</h2>
    </div>
    <div class="meta">
      <div><strong>مكان الاستخدام:</strong> ${item.location}</div>
      <div><strong>قناة الإرسال:</strong> ${item.channel}</div>
    </div>
    <div class="content-grid">
      <div class="lang-section">
        <div class="lang-title">الرسالة باللغة العربية</div>
        <pre dir="rtl">${item.bodyAR}</pre>
      </div>
      <div class="lang-section" dir="ltr">
        <div class="lang-title">الرسالة باللغة الإنجليزية</div>
        <pre dir="ltr">${item.bodyEN}</pre>
      </div>
    </div>
  </div>`;
  counter++;
}

html += `</body></html>`;

fs.writeFileSync('c:/Users/t-a.jaber/RiderProjects/Tawtheef/src/notifications_summary.html', "\uFEFF" + html, 'utf8');
console.log('HTML written successfully.');
