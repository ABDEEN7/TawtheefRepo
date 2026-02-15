const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

function gitShortSha() {
  try { return execSync('git rev-parse --short HEAD').toString().trim(); }
  catch { return 'no-git'; }
}

const outPath = path.join(__dirname, '..', 'src', 'assets', 'version.json');

const buildNo =
  process.env.BUILD_NO ||
  process.env.GITHUB_RUN_NUMBER ||
  process.env.BUILD_BUILDID ||
  Date.now().toString(); // fallback for local/FTP builds

const data = {
  version: process.env.APP_VERSION || `1.0.${buildNo}`,
  commit: process.env.APP_COMMIT || gitShortSha(),
  buildNo: Number(buildNo) || buildNo,
  builtAt: new Date().toISOString(),
  env: process.env.APP_ENV || 'production'
};

fs.mkdirSync(path.dirname(outPath), { recursive: true });
fs.writeFileSync(outPath, JSON.stringify(data, null, 2), 'utf8');

console.log('✅ version.json written to', outPath);
console.log(data);
