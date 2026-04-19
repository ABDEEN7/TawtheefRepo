const fs = require('fs');
const path = require('path');

const baseDir = 'c:/Users/t-a.jaber/RiderProjects/Tawtheef/src/Notifications/Templates';

function walk(dir) {
    let results = [];
    const list = fs.readdirSync(dir);
    list.forEach(file => {
        file = path.resolve(dir, file);
        const stat = fs.statSync(file);
        if (stat && stat.isDirectory()) {
            results = results.concat(walk(file));
        } else {
            if (file.endsWith('.cshtml')) {
                results.push(file);
            }
        }
    });
    return results;
}

const files = walk(baseDir);

files.forEach(file => {
    let content = fs.readFileSync(file, 'utf8');
    let original = content;

    // Pattern for quoted expression: "@(...)"
    const pattern1 = /\"@\(string\.IsNullOrWhiteSpace\(Model\.Model\.JobTitle\)\s*\?\s*\"[^\"]*\"\s*:\s*Model\.Model\.JobTitle\)\"/g;
    content = content.replace(pattern1, '@(string.IsNullOrWhiteSpace(Model.Model.JobTitle) ? "" : $" \\"{Model.Model.JobTitle}\\"")');

    // Pattern for <strong>...</strong>
    const pattern2 = /<strong>@\(string\.IsNullOrWhiteSpace\(Model\.Model\.JobTitle\)\s*\?\s*\"[^\"]*\"\s*:\s*Model\.Model\.JobTitle\)<\/strong>/g;
    content = content.replace(pattern2, '@(string.IsNullOrWhiteSpace(Model.Model.JobTitle) ? "" : $" <strong>{Model.Model.JobTitle}</strong>")');

    // Pattern for plain expression: @(...)
    const pattern3 = /@\(string\.IsNullOrWhiteSpace\(Model\.Model\.JobTitle\)\s*\?\s*\"[^\"]*\"\s*:\s*Model\.Model\.JobTitle\)/g;
    content = content.replace(pattern3, '@Model.Model.JobTitle');

    if (content !== original) {
        fs.writeFileSync(file, content, 'utf8');
        console.log('Fixed: ' + file);
    }
});
