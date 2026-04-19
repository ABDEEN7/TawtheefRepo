const fs = require('fs');

const csprojPath = 'c:/Users/t-a.jaber/RiderProjects/Tawtheef/src/Notifications/Notifications.csproj';
let content = fs.readFileSync(csprojPath, 'utf8');

// The ItemGroups structure is quite repetitive. Let's just remove all ItemGroups
// that contain "Templates\" references and append a clean one.

const lines = content.split(/\r?\n/);
let newLines = [];
let skipGroup = false;

for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    
    // Check if line enters an ItemGroup
    if (line.includes('<ItemGroup>')) {
        // Look ahead to see if this ItemGroup contains "Templates\"
        let hasTemplatesBlock = false;
        for (let j = i + 1; j < lines.length; j++) {
            if (lines[j].includes('</ItemGroup>')) break;
            if (lines[j].includes('Templates\\')) {
                hasTemplatesBlock = true;
                break;
            }
        }
        
        if (hasTemplatesBlock) {
            skipGroup = true;
            continue;
        }
    }
    
    if (skipGroup) {
        if (line.includes('</ItemGroup>')) {
            skipGroup = false;
        }
        continue;
    }
    
    // Check if line itself contains Templates\ but not in an ItemGroup (shouldn't happen but just in case)
    if (!line.includes('Templates\\')) {
        newLines.push(line);
    }
}

// Ensure the closure of the Project
while(newLines[newLines.length - 1] && (newLines[newLines.length - 1].includes('</Project>') || newLines[newLines.length - 1].trim() === '')) {
    newLines.pop();
}

const itemGroupTemplates = `
    <ItemGroup>
        <None Remove="Templates\\**\\*.cshtml" />
        <EmbeddedResource Include="Templates\\**\\*.cshtml">
            <CopyToOutputDirectory>Always</CopyToOutputDirectory>
        </EmbeddedResource>
    </ItemGroup>
</Project>`;

fs.writeFileSync(csprojPath, newLines.join('\n') + '\n' + itemGroupTemplates, 'utf8');
console.log('csproj updated');
