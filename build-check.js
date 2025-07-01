const fs = require('fs');
const path = require('path');

console.log('🔍 Verificando configuración del proyecto...');

const clientPath = path.join(__dirname, 'HealthPredict.Client');
const requiredFiles = [
  'package.json',
  'angular.json',
  'tsconfig.json',
  'tsconfig.app.json',
  'src/main.ts',
  'src/app/app.module.ts',
  'src/app/app.component.ts'
];

let allFilesExist = true;

requiredFiles.forEach(file => {
  const filePath = path.join(clientPath, file);
  if (fs.existsSync(filePath)) {
    console.log(`✅ ${file}`);
  } else {
    console.log(`❌ ${file} - FALTA`);
    allFilesExist = false;
  }
});

if (allFilesExist) {
  console.log('\n✅ Todos los archivos necesarios están presentes');
  console.log('🚀 El proyecto debería compilar correctamente');
} else {
  console.log('\n❌ Faltan archivos necesarios');
  process.exit(1);
}

// Verificar package.json
const packageJsonPath = path.join(clientPath, 'package.json');
if (fs.existsSync(packageJsonPath)) {
  const packageJson = JSON.parse(fs.readFileSync(packageJsonPath, 'utf8'));
  console.log('\n📦 Información del paquete:');
  console.log(`   Nombre: ${packageJson.name}`);
  console.log(`   Versión: ${packageJson.version}`);
  console.log(`   Angular CLI: ${packageJson.devDependencies['@angular/cli']}`);
  console.log(`   Angular Core: ${packageJson.dependencies['@angular/core']}`);
} 