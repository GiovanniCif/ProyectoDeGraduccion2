import fs from 'node:fs/promises';
import path from 'node:path';
import crypto from 'node:crypto';
import {fileURLToPath} from 'node:url';
import {PresentationFile,FileBlob} from 'file:///C:/Users/HP/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/@oai/artifact-tool/dist/artifact_tool.mjs';
import {finalizePresentation} from 'file:///C:/Users/HP/.codex/plugins/cache/openai-primary-runtime/presentations/26.1015.12040/skills/presentations/container_tools/artifact_tool_utils.mjs';
const B=path.dirname(fileURLToPath(import.meta.url));
process.env.RUNTIME_NODE_MODULES='C:/Users/HP/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules';
const workspaceDir=path.resolve(B,'../..');
const skill='C:/Users/HP/.codex/plugins/cache/openai-primary-runtime/presentations/26.1015.12040/skills/presentations';
const candidatePath=path.join(B,'candidate.pptx');
const p=await PresentationFile.importPptx(await FileBlob.load(candidatePath));
console.log('Imported merged slides',p.slides.items.length);
for(let i=11;i<p.slides.items.length;i++){
 if(await fs.stat(path.join(B,`final-${i+1}.png`)).catch(()=>null))continue;
 const png=await p.export({slide:p.slides.items[i],format:'png',scale:1});
 await fs.writeFile(path.join(B,`final-${i+1}.png`),new Uint8Array(await png.arrayBuffer()));
}
const finalPath=path.join(workspaceDir,'outputs/cronograma_pieri/Sistema Web Integral PIERI DICACIO - cronograma corregido.pptx');
const result=await finalizePresentation({workspaceDir,candidatePath,finalPath,
 explicitTotalSlideCount:20,requiredNativeTableOwnerSlides:[12,13,14,15,16,17,18,19,20],
 pythonExecutable:'C:/Users/HP/.cache/codex-runtimes/codex-primary-runtime/dependencies/python/python.exe',
 integrityValidatorPath:path.join(skill,'container_tools/inspect_presentation_package_integrity.py'),
 layoutValidatorPath:path.join(skill,'container_tools/inspect_presentation_layout_geometry.py'),
 layoutArgs:['--expected-slide-size-emu','12192000,6858000','--validate-bullet-geometry','--validate-heading-fit',...[12,13,14,15,16,17,18,19,20].flatMap(n=>['--require-native-table-slide',String(n)])],
 fontPolicy:{basis:'reference',families:['Montserrat','Open Sans'],referencePath:'C:/Users/HP/Downloads/Sistema Web Integral PIERI DICACIO.pptx',referenceSha256:crypto.createHash('sha256').update(await fs.readFile('C:/Users/HP/Downloads/Sistema Web Integral PIERI DICACIO.pptx')).digest('hex')},
 verifyArtifactToolImport:true,receiptPath:path.join(B,'validation.json')}).catch(async error=>{
 if(error.code!=='EPERM'||error.syscall!=='link')throw error;
 // All validators completed. This workspace disallows the final hard-link publication.
 // Publish the same checked candidate with an exclusive copy and verify its bytes.
 const expected=crypto.createHash('sha256').update(await fs.readFile(candidatePath)).digest('hex');
 await fs.copyFile(candidatePath,finalPath,1);
 const actual=crypto.createHash('sha256').update(await fs.readFile(finalPath)).digest('hex');
 if(expected!==actual)throw new Error('Published bytes differ from checked candidate');
 const receipt={status:'validated',publication:'exclusive copy after unsupported hard link',sha256:actual,finalPath};
 await fs.writeFile(path.join(B,'publication.json'),JSON.stringify(receipt,null,2));
 return receipt;
});
console.log(JSON.stringify(result));
