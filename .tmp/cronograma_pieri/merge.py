from pathlib import Path
import xml.etree.ElementTree as E, zipfile, posixpath as P, json, re
B=Path(__file__).parent
R='http://schemas.openxmlformats.org/package/2006/relationships'
O='http://schemas.openxmlformats.org/officeDocument/2006/relationships'
S='http://schemas.openxmlformats.org/presentationml/2006/main'
A='http://schemas.openxmlformats.org/drawingml/2006/main'
C='http://schemas.openxmlformats.org/package/2006/content-types'
E.register_namespace('p',S);E.register_namespace('a',A);E.register_namespace('r',O)
src=zipfile.ZipFile(r'C:\Users\HP\Downloads\Sistema Web Integral PIERI DICACIO.pptx')
gen=zipfile.ZipFile(B/'schedule.pptx')
out={n:src.read(n) for n in src.namelist()}
root=E.fromstring(out['ppt/presentation.xml']); rels=E.fromstring(out['ppt/_rels/presentation.xml.rels']);ct=E.fromstring(out['[Content_Types].xml'])
gr=E.fromstring(gen.read('ppt/presentation.xml'));grels=E.fromstring(gen.read('ppt/_rels/presentation.xml.rels'))
resolve=lambda base,target:P.normpath(P.join(P.dirname(base),target)) if not target.startswith('/') else target[1:]
rp=lambda part:P.join(P.dirname(part),'_rels',P.basename(part)+'.rels')
orig_notes=next(resolve('ppt/presentation.xml',r.get('Target')) for r in rels if r.get('Type').endswith('/notesMaster'))
types={x.get('PartName')[1:]:x.get('ContentType') for x in E.fromstring(gen.read('[Content_Types].xml')) if x.tag.endswith('Override')}
mapping={}; added=set()
def cp(old):
 if old in mapping:return mapping[old]
 sm=re.fullmatch(r'ppt/slides/slide(\d+)\.xml',old)
 new=f'ppt/slides/slide{int(sm.group(1))+12}.xml' if sm else 'ppt/pieri_schedule/'+old.removeprefix('ppt/')
 mapping[old]=new
 out[new]=gen.read(old);added.add(old)
 if old in types and not any(x.get('PartName')=='/'+new for x in ct):E.SubElement(ct,'{'+C+'}Override',{'PartName':'/'+new,'ContentType':types[old]})
 rr=rp(old)
 if rr in gen.namelist():
  rroot=E.fromstring(gen.read(rr))
  for r in rroot:
   if r.get('TargetMode')=='External':continue
   target=resolve(old,r.get('Target'))
   dest=orig_notes if r.get('Type').endswith('/notesMaster') else cp(target)
   r.set('Target',P.relpath(dest,P.dirname(new)))
  E.register_namespace('',R)
  out[rp(new)]=E.tostring(rroot,encoding='utf-8',xml_declaration=True)
 return new
lst=root.find('{'+S+'}sldIdLst'); remove=list(lst)[12:]
remove_ids={x.get('{'+O+'}id') for x in remove}
for x in remove:lst.remove(x)
for r in list(rels):
 if r.get('Id') in remove_ids:rels.remove(r)
nextid=max(int(x.get('id')) for x in lst)+1
def add_rel(target,kind):
 rid='rIdSchedule'+str(len(rels)+1)
 E.SubElement(rels,'{'+R+'}Relationship',{'Id':rid,'Type':O+'/'+kind,'Target':P.relpath(target,'ppt')})
 return rid
gmap={r.get('Id'):r for r in grels}
for el in gr.find('{'+S+'}sldIdLst'):
 rel=gmap[el.get('{'+O+'}id')];part=resolve('ppt/presentation.xml',rel.get('Target'));dest=cp(part)
 rid=add_rel(dest,'slide');E.SubElement(lst,'{'+S+'}sldId',{'id':str(nextid),'{'+O+'}id':rid});nextid+=1
ml=root.find('{'+S+'}sldMasterIdLst');mid=max([int(x.get('id')) for x in ml] or [2147483647])+1
for r in grels:
 if r.get('Type').endswith('/slideMaster'):
  old=resolve('ppt/presentation.xml',r.get('Target'))
  if old in mapping:
   rid=add_rel(mapping[old],'slideMaster');E.SubElement(ml,'{'+S+'}sldMasterId',{'id':str(mid),'{'+O+'}id':rid});mid+=1
defaults={x.get('Extension') for x in ct if x.tag.endswith('Default')}
for el in E.fromstring(gen.read('[Content_Types].xml')):
 if el.tag.endswith('Default') and el.get('Extension') not in defaults:ct.append(el);defaults.add(el.get('Extension'))
out['ppt/presentation.xml']=E.tostring(root,encoding='utf-8',xml_declaration=True)
E.register_namespace('',R)
out['ppt/_rels/presentation.xml.rels']=E.tostring(rels,encoding='utf-8',xml_declaration=True)
E.register_namespace('',C)
out['[Content_Types].xml']=E.tostring(ct,encoding='utf-8',xml_declaration=True)
# Keep the original iteration table, updating only its six period cells.
sl=E.fromstring(out['ppt/slides/slide12.xml']); rows=sl.findall('.//{'+A+'}tr')
tasks=json.loads((B/'tasks.json').read_text(encoding='utf8'));summ=[t for t in tasks if t['OutlineLevel']=='1'][:6]
for row,t in zip(rows[1:],summ):
 cell=row.findall('{'+A+'}tc')[1]; runs=cell.findall('.//{'+A+'}t')
 fmt=lambda x:x[8:10]+'/'+x[5:7]+'/'+x[2:4]
 runs[0].text=fmt(t['Start'])+' – '+fmt(t['Finish'])
 for x in runs[1:]:x.text=''
out['ppt/slides/slide12.xml']=E.tostring(sl,encoding='utf-8',xml_declaration=True)
if 'docProps/app.xml' in out:
 app=E.fromstring(out['docProps/app.xml']);ns='http://schemas.openxmlformats.org/officeDocument/2006/extended-properties';count=app.find('{'+ns+'}Slides')
 if count is not None:count.text='20'
 out['docProps/app.xml']=E.tostring(app,encoding='utf-8',xml_declaration=True)
with zipfile.ZipFile(B/'candidate.pptx','w',zipfile.ZIP_DEFLATED) as z:
 for name,data in out.items():z.writestr(name,data)
assert all(out[f'ppt/slides/slide{i}.xml']==src.read(f'ppt/slides/slide{i}.xml') for i in range(1,12))
print(json.dumps({'slides':len(lst),'originalSlidesPreserved':11,'scheduleTasks':len(tasks)}))
