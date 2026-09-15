export const POWER_NAMES = ['Magnet','Score booster','Shield','Speed burst','Double tokens','Air dash','Hover board','Slow motion','Invincibility'];
export function safeLane(seed, row) { let n = (seed + Math.imul(row,374761393)) >>> 0; n = Math.imul(n ^ (n >>> 13),1274126177) >>> 0; return ((n ^ (n >>> 16)) >>> 0) % 3 - 1; }
export class Game {
  constructor(random = Math.random) { this.random=random; this.state='menu'; this.items=[]; this.distance=0; this.x=0; this.height=0; this.slide=0; this.powers={}; this.coins=0; this.points=0; this.events=[]; }
  start(upgrade=0) { Object.assign(this,{state:'running',distance:0,x:0,lane:0,height:0,velocity:0,slide:0,coins:0,points:0,immunity:2,chase:0,revived:false,banked:false,row:0,nextRow:42,items:[],powers:{},events:[],upgrade,seed:Math.floor(this.random()*1000000)}); this.generate(); }
  get score(){ return Math.floor(this.points)+this.coins*10; }
  get speed(){return Math.min(30,12+this.distance/240)*(this.powers[3]>0?1.3:this.powers[7]>0?.65:1);}
  action(action){if(action==='pause'){if(this.state==='running')this.state='paused';else if(this.state==='paused')this.state='running';return;} if(this.state!=='running')return;
    if(action==='left')this.lane=Math.max(-1,this.lane-1);if(action==='right')this.lane=Math.min(1,this.lane+1);
    if(action==='jump'&&this.height<=.08){this.slide=0;this.velocity=Math.sqrt(2*26*2.6);this.events.push('jump');}
    if(action==='slide'){this.slide=.8;if(this.height>.1)this.velocity=-16;}
  }
  generate(){while(this.nextRow < this.distance+220){const safe=safeLane(this.seed,this.row);for(let lane=-1;lane<=1;lane++){if(lane!==safe)this.items.push({kind:['barrier','gate','cargo','vehicle','pipe'][(this.row+lane+6)%5],lane,z:this.nextRow,done:false});}for(let c=0;c<5;c++)this.items.push({kind:'coin',lane:safe,z:this.nextRow+c*1.5,done:false});if(this.row%5===4)this.items.push({kind:'power',power:Math.floor(this.row/5)%9,lane:safe,z:this.nextRow+8,done:false});this.row++;this.nextRow+=18;}}
  hit(major){if(this.immunity>0)return;if(this.powers[8]>0)return;for(const id of [2,6])if(this.powers[id]>0){delete this.powers[id];this.immunity=1.2;this.events.push('Protected!');return;}if(!major&&this.chase<.6){this.chase=1;this.immunity=1;this.events.push('Patrol close — keep clear!');return;}this.state='over';this.events.push('crash');}
  revive(){if(this.state!=='over'||this.revived)return;this.revived=true;this.immunity=3;this.chase=0;this.items=this.items.filter(i=>Math.abs(i.z-this.distance)>12);this.state='running';}
  tick(dt){if(this.state!=='running')return;dt=Math.min(.05,Math.max(0,dt));const before=this.distance,meters=this.speed*dt;this.points+=meters*(1+Math.min(4,Math.floor(this.distance/500))+(this.powers[1]>0?2:0));this.distance+=meters;this.immunity=Math.max(0,this.immunity-dt);this.chase=Math.max(0,this.chase-dt*.09);for(const id of Object.keys(this.powers)){this.powers[id]-=dt;if(this.powers[id]<=0)delete this.powers[id];}
    this.x+=(this.lane*2.7-this.x)*(1-Math.exp(-15*dt));this.slide=Math.max(0,this.slide-dt);this.velocity-=26*dt;this.height=Math.max(0,this.height+this.velocity*dt);if(this.height===0)this.velocity=0;
    for(const item of this.items){if(item.done)continue;const z=item.z-this.distance,previous=item.z-before;const near=Math.abs(item.lane*2.7-this.x)<.9;
      if(item.kind==='coin'){if((this.powers[0]>0&&Math.abs(z)<9)||(near&&previous>=-.8&&z<=.8&&this.height<1.4)){item.done=true;this.coins+=this.powers[4]>0?2:1;this.events.push('coin');}}
      else if(near&&previous>=-.75&&z<=(item.kind==='vehicle'?2.7:.75)){item.done=true;if(item.kind==='power'){this.powers[item.power]=8+this.upgrade;this.events.push(POWER_NAMES[item.power]+' activated');}else{const clear=this.powers[5]>0||((item.kind==='barrier'&&this.height>1.15)||(item.kind==='pipe'&&this.height>.65)||(item.kind==='gate'&&this.slide>0&&this.height<.2));if(!clear)this.hit(item.kind!=='pipe');}}
      if(this.state!=='running')break;
    }this.items=this.items.filter(i=>i.z>this.distance-15);this.generate();
  }
}
