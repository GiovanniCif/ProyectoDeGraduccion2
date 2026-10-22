import zipfile, xml.etree.ElementTree as E, json, re
from pathlib import Path
B=Path(__file__).parent
ns={'m':'http://schemas.microsoft.com/project'}
root=E.parse(r'C:\Users\HP\OneDrive\Documentos\U segundo semestre 2026\proyecto final 1\Cronograma proyecto de graduacion excel .xml').getroot()
tasks=[]
for t in root.findall('m:Tasks/m:Task',ns):
 d={k:t.findtext('m:'+k,default='',namespaces=ns) for k in ['UID','ID','Name','OutlineLevel','Start','Finish','Duration','PercentComplete']}
 if d['Name']:tasks.append(d)
(B/'tasks.json').write_text(json.dumps(tasks,ensure_ascii=False,indent=2),encoding='utf8')
n={'a':'http://schemas.openxmlformats.org/drawingml/2006/main','p':'http://schemas.openxmlformats.org/presentationml/2006/main'}
with zipfile.ZipFile(r'C:\Users\HP\Downloads\Sistema Web Integral PIERI DICACIO.pptx') as z:
 for i in [12,13,14]:
  r=E.fromstring(z.read(f'ppt/slides/slide{i}.xml'))
  print('SLIDE',i)
  for sp in r.findall('.//p:sp',n):
   texts=[t.text for t in sp.findall('.//a:t',n)]
   pos=sp.find('p:spPr/a:xfrm',n)
   if texts:print(json.dumps({'text':texts,'pos':E.tostring(pos,encoding='unicode') if pos is not None else '', 'fonts':[e.attrib for e in sp.findall('.//a:rPr',n)][:1]},ensure_ascii=False))
  print('BG',E.tostring(r.find('p:cSld/p:bg',n),encoding='unicode') if r.find('p:cSld/p:bg',n) is not None else '')
