$fn = 50;

// Height (z) of the base board
baseHeight = 2;

// Make the module sections stick out a little from the base so 
// they are easily ifentified.
moduleHeight = baseHeight + 0.5;

module PlantStand() {
	difference() {
		union() {
			cylinder(h = baseHeight+20,r = 50);
		}
		union() {
			translate(v=[0, 0,3]) {
				cylinder(h = baseHeight+20,r = 48);
			}
		}
	}
}

/////////////////////////////////////////////////////////////////////
//
/////////////////////////////////////////////////////////////////////
module BaseBoard(x,y,height,width) {
	translate([x, y, 0]) {
		cube([height, width,baseHeight]);
	}
}

module BackMountCutout(x,y,length) {
	translate([x, y, 0]) {
		difference() {
			union() {
				//cube([length, 10,10]);
			}
			
		}
	}
}

module Mount(x,y,length, height=15) {
	translate([x, y, 0]) {
		difference() {
			union() {
				cube([4, length,height]);
			}
			union() {
				rotate([90,90,90]) {
					translate([x-50,y+50,-5]) {
						//#cylinder(r=1.25, h=15, center=true, $fn=50);	
					}
					translate([x,y+100,5]) {
						//#cylinder(r=1.25, h=15, center=true, $fn=50);	
					}
				}				
			}
		}
	}
}

module PumpPin(x,y) {
	translate([x, y, (baseHeight + 10)/2]) {
		cylinder(r=2, h=baseHeight + 10, center=true, $fn=50);	
	}
}

module PumpPins() {

	translate([95, -20,0]) {
		PumpPin(0,0);
		PumpPin(20,0);
		PumpPin(20,43);
		PumpPin(0,43);
	}
}

/////////////////////////////////////////////////////////////////////
// Main
/////////////////////////////////////////////////////////////////////

difference() {
	union() {
		BaseBoard(0, -50,130, 100);
		PlantStand();
		Mount(52,-50,100);
		Mount(66.25,-50,100, 12);
		PumpPins();
	}
	union() {
		BackMountCutout(70,-50);
	}
}

