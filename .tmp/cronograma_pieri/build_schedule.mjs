import fs from 'node:fs/promises';
import {fileURLToPath} from 'node:url';
import {Presentation,PresentationFile} from 'file:///C:/Users/HP/.cache/codex-runtimes/codex-primary-runtime/dependencies/node/node_modules/@oai/artifact-tool/dist/artifact_tool.mjs';
const B=new URL('./',import.meta.url),tasks=JSON.parse(await fs.readFile(new URL('tasks.json',B),'utf8'));
const p=Presentation.create({slideSize:{width:1280,height:720}});
const C={wine:'#7B1114',orange:'#D96B10',gray:'#4D5866',ivory:'#FAF7F2',beige:'#F3E7DA',white:'#FFFFFF'};
const DAY=86400000,date=s=>Date.parse(s.slice(0,10)+'T00:00:00Z'),dt=s=>s.slice(8,10)+'/'+s.slice(5,7),dur=s=>Number(s.match(/PT([\d.]+)H/)?.[1]||0)/8;
function text(s,t,x,y,w,h,size=20,color=C.gray,bold=false,font='Open Sans',align='left'){
 const a=s.shapes.add({geometry:'textbox',name:t.slice(0,45),position:{left:x,top:y,width:w,height:h},fill:'none',line:{fill:'none',width:0}});
 a.text=t;a.text.style={fontSize:size,typeface:font,color,bold,alignment:align,insets:0,autoFit:'none',wrap:'square'};return a;
}
function rect(s,x,y,w,h,fill){return s.shapes.add({geometry:'rect',position:{left:x,top:y,width:w,height:h},fill,line:{fill:'none',width:0}});}
const groups=[];
for(const t of tasks){if(t.OutlineLevel==='1')groups.push({head:t,children:[]});else groups.at(-1).children.push(t);}
const titles=['Usuarios y roles','Gestión y control de pedidos','Gestión integral de inventarios','Control de pagos','Asistencia del personal','Reportes y dashboard','Integración, implementación y entrega'];
function page(title,rows,start,end,idx,detail=false){
 const s=p.slides.add();s.background.fill=C.ivory;
 text(s,title,85.33,85.33,1110,62,42.667,C.wine,true,'Montserrat');
 const sub=detail?`${dt(start)} al ${dt(end)} de 2026  ·  ${dur(groups[idx-1].head.Duration)} días laborables`:'16 de marzo al 9 de octubre de 2026';
 text(s,sub,85.33,151,1110,31,21);
 const top=229,rh=57,X=859,W=335,left=85,widths=[50,428,62,103,103];
 const vals=[['ID','Nombre de tarea','Dur.','Inicio','Fin'],...rows.map(t=>[t.ID,t.Name,`${dur(t.Duration)} d`,dt(t.Start)+'/26',dt(t.Finish)+'/26'])];
 const table=s.tables.add({rows:vals.length,columns:5,left,top:top-42,width:746,height:42+rows.length*rh,columnWidths:widths,values:vals});
 table.rows[0].height=42;for(let r=1;r<vals.length;r++)table.rows[r].height=rh;
 table.borders.assign({fill:C.beige,width:1,style:'solid'});
 for(let r=0;r<vals.length;r++)for(let c=0;c<5;c++){
  let cell=table.getCell(r,c);cell.fill=r===0?C.wine:C.white;
  cell.text.style={fontSize:r===0?18:(c>=3?17:18),typeface:'Open Sans',color:r===0?C.white:C.gray,bold:r===0||(!detail&&c===1),insets:5,verticalAlignment:'middle',alignment:c===1?'left':'center'};
 }
 const a=date(start),b=date(end)+DAY,xx=n=>X+(n-a)/(b-a)*W;
 rect(s,X,top-42,W,42,C.beige);
 const ticks=[];
 if(detail){for(let d=a;d<b;d+=7*DAY)ticks.push(d);}else{for(let m=2;m<10;m++){let d=Date.UTC(2026,m,1);if(d<a)d=a;if(d<b)ticks.push(d);}}
 ticks.forEach((d,i)=>{const next=ticks[i+1]??b;const label=detail?dt(new Date(d).toISOString()):['ene','feb','mar','abr','may','jun','jul','ago','sep','oct'][new Date(d).getUTCMonth()];text(s,label,xx(d)+1,top-32,Math.max(25,xx(next)-xx(d)-2),26,detail?16:15,C.wine,true,'Open Sans','center');});
 rows.forEach((t,i)=>{
  const y=top+i*rh;rect(s,X,y,W,rh,C.white);
  if(detail)for(let d=a;d<b;d+=DAY){if([0,6].includes(new Date(d).getUTCDay()))rect(s,xx(d),y,W*DAY/(b-a),rh,C.beige);}
  ticks.forEach(d=>rect(s,xx(d),y,0.7,rh,C.beige));
  const sx=xx(date(t.Start)),ex=xx(date(t.Finish)+DAY);rect(s,sx,y+18,Math.max(2,ex-sx),21,C.orange);
  rect(s,X,y+rh-1,W,1,C.beige);
 });
 text(s,'Duraciones según el XML: jornada de 8 h. Barras = período programado, no porcentaje de avance.',85,648,1110,25,16);
 text(s,`Cronograma XML 2026`,85,680,800,20,14,C.gray);text(s,`${13+idx} / 20`,1100,680,94,22,14,C.wine,false,'Open Sans','right');
 s.speakerNotes.text='Fuente: Cronograma proyecto de graduacion excel .xml aportado por el usuario. Se conservan ID, nombres completos, fechas y duración de cada tarea. Duración en días = horas XML / 8, según MinutesPerDay=480. Inicio de las tareas a las 08:00 y fin a las 17:00 según XML. El eje del Gantt representa días calendario con fin inclusivo. El XML conserva 0 % de avance.\n'+rows.map(t=>JSON.stringify(t)).join('\n');
 return s;
}
page('Cronograma de implementación 2026',groups.map(g=>g.head),tasks[0].Start,tasks.at(-1).Finish,0);
groups.forEach((g,i)=>page(`${i+1}. ${titles[i]}`,g.children,g.head.Start,g.head.Finish,i+1,true));
for(let i=0;i<p.slides.items.length;i++){
 const s=p.slides.items[i];const png=await p.export({slide:s,format:'png',scale:1});
 await fs.writeFile(new URL(`new-${13+i}.png`,B),new Uint8Array(await png.arrayBuffer()));
 const layout=await s.export({format:'layout'});await fs.writeFile(new URL(`new-${13+i}.layout.json`,B),await layout.text());
}
await(await PresentationFile.exportPptx(p)).save(fileURLToPath(new URL('schedule.pptx',B)));
console.log(JSON.stringify({newSlides:p.slides.items.length,tasks:tasks.length,details:groups.reduce((n,g)=>n+g.children.length,0)}));
