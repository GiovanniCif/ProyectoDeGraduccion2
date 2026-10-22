import fs from 'node:fs/promises';
import {Presentation,PresentationFile,FileBlob} from 'file:///C:/Users/HP/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/@oai/artifact-tool/dist/artifact_tool.mjs';
const B=new URL('./',import.meta.url);
const p=await PresentationFile.importPptx(await FileBlob.load('C:/Users/HP/Downloads/Sistema Web Integral PIERI DICACIO.pptx'));
for(const i of [11,12,13]){
 const b=await p.export({slide:p.slides.items[i],format:'png',scale:1});
 await fs.writeFile(new URL(`source-${i+1}.png`,B),new Uint8Array(await b.arrayBuffer()));
}
await fs.writeFile(new URL('source-proto.json',B),JSON.stringify(p.toProto()));
console.log('Imported',p.slides.items.length);
