$fn = 50;

// Height (z) of the base board
baseHeight = 2;

// Make the module sections stick out a little from the base so 
// they are easily ifentified.
moduleHeight = baseHeight + 0.5;



// a 6mm pad with 2.5mm inner for a 3mm self tapping screw.
// positioned at x,y
module Pad(x,y, spacerHeight = 5) {
	translate([x,y,spacerHeight/2]) {
		difference() {
			cylinder(r=3.5, h=spacerHeight, center=true, $fn=50);
			// subtract the inner part of the pad for a screw to fit in.
			// extends above the pad to ensure nothing fowls it.
			// is 1mm short of going through so that it doesn't make holes in the base board.
			translate([0,0,0]) {
				#cylinder(r=1.25, h=spacerHeight+2, center=true, $fn=50);
			}
		}
	}
}

module PinPad(x,y, spacerHeight = 5) {
	translate([x,y,spacerHeight/2]) {
		union() {
			cylinder(r=3.5, h=spacerHeight, center=true, $fn=50);
			cylinder(r=1.25, h=spacerHeight+4, center=true, $fn=50);
		}
	}
}

/////////////////////////////////////////////////////////////////////
// Generic Module
/////////////////////////////////////////////////////////////////////
module GenericGadgeteerModule(x,y, xDistance, yDistance, rotation=0) {
	// Typically board is +3.5mm wider than the x/y distance between pads.
	translate([x,y,0]) {
		rotate(rotation) {
			GenericGadgeteerBase(xDistance, yDistance);
			GenericGadgeteerPads(xDistance, yDistance);
		}
	}
}

module GenericGadgeteerBase(xDistance, yDistance) {
	// Base is typically 3.5mm larger on each side than the hole spacing
	// extend the base out by 3.5 from holes by using minkowski
	// which gives rounded corners to the board in the process
	// matching the Gadgeteer design
	// Use 3.5mm radious cylinder to match the mounting pads.
	
   cube([xDistance, yDistance, moduleHeight]);
	minkowski()
	{
		cube([xDistance, yDistance, moduleHeight/2]);
		cylinder(r=3.5,h=moduleHeight/2);
	}
}

// Place pads for Generic Gadgeteer Module
// xDistance/yDistance - distance between x and y pads.
module GenericGadgeteerPads(xDistance, yDistance) {

	translate([0,0,moduleHeight]) {
		Pad(0,0);
		PinPad(xDistance,0);
		Pad(xDistance,yDistance);
		PinPad(0,yDistance);
	}
}

/////////////////////////////////////////////////////////////////////
// End generic
/////////////////////////////////////////////////////////////////////


/////////////////////////////////////////////////////////////////////
// Fez Cerberus
/////////////////////////////////////////////////////////////////////
// x,y is top left pad center.
// board extends -3.5 in x and y and +3.5
module FezCerberus(x,y,rotation) {
	GenericGadgeteerModule(x,y,40,50,rotation);
}


/////////////////////////////////////////////////////////////////////
// Fez Hydra
/////////////////////////////////////////////////////////////////////
// x,y is top left pad center.
// board extends -3.5 in x and y and +3.5
module FezHydra(x,y,rotation) {
	GenericGadgeteerModule(x,y,55,80,rotation);
}

/////////////////////////////////////////////////////////////////////
// RS21 Wifi
/////////////////////////////////////////////////////////////////////
module RS21Wifi(x,y,rotation) {
	GenericGadgeteerModule(x,y,35,35,rotation);
}

/////////////////////////////////////////////////////////////////////
// ENC28 Ethernet Module
/////////////////////////////////////////////////////////////////////
module ENC28(x,y,rotation) {
	GenericGadgeteerModule(x,y,35,30,rotation);
}

/////////////////////////////////////////////////////////////////////
// 
/////////////////////////////////////////////////////////////////////
module RelayX1(x,y,rotation) {
	GenericGadgeteerModule(x,y,30,30,rotation);
}

/////////////////////////////////////////////////////////////////////
// N18 Display
/////////////////////////////////////////////////////////////////////
module N18Display(x,y) {
	translate([x,y,0]) {
		N18DisplayBase();
		N18DisplayPads();
	}
}

module N18DisplayBase() {
	translate([-3.5,-3.5,0]) {
		cube([34,55, moduleHeight]);
	}
}

module N18DisplayPads() {
	translate([0,0,moduleHeight]) {
		Pad(0,0);
		Pad(25,0);
	}
}

module N18DisplayCutOut(x,y) {
	translate([x + (28.9/2), y+(35.9/2) + 10,-3 + moduleHeight]) {
		#cube([28.9, 35.9,10], center=true);
	}
}

/////////////////////////////////////////////////////////////////////
// Usb Client DP
/////////////////////////////////////////////////////////////////////
module UsbClientDP(x, y, rotation) {
	GenericGadgeteerModule(x,y,30,35, rotation);
}


/////////////////////////////////////////////////////////////////////
// Light Sensor
/////////////////////////////////////////////////////////////////////
module LightSensor(x,y) {
	translate([x,y,0]) {
		GenericGadgeteerBase(10, 13);
		LightSensorPads();
	}
}

module LightSensorPads(x,y) {
	translate([0,0,moduleHeight]) {
		Pad(0,0, 2);
		Pad(10,0, 2);
	}
}

module LightSensorCutOut(x,y) {
	translate([x + 1, y+6.5, -9 + moduleHeight]) {
		#cylinder(r=4, h=20, center=true, $fn=50);
	}
}

/////////////////////////////////////////////////////////////////////
// Moisture Sensor CutOut
/////////////////////////////////////////////////////////////////////
// hole for moisture sensor cable to go through.
module MoistureSensorCutOut(x,y) {
	translate([x, y,-2]) {
		#cube([15,5,4 + baseHeight]);
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

module Mount(x,y,length) {
	translate([x, y, 0]) {
		difference() {
			union() {
				cube([length, 10,10]);
			}
			union() {
				
				translate([10,5,5]) {
					rotate([0,90,90]) {
						#cylinder(r=1.25, h=15, center=true, $fn=50);	
					}
				}
				translate([100,5,5]) {
					rotate([0,90,90]) {
						#cylinder(r=1.25, h=15, center=true, $fn=50);	
					}
				}

			}
		}
	}
}

module Top(x,y,length) {
	translate([x, y, 0]) {
		difference() {
			union() {
				cube([length, 2,15]);
			}
			union() {
			}
		}
	}
}



// Base
//poly_rect3826(3);
lightSensorY = 10;

/////////////////////////////////////////////////////////////////////
// Main
/////////////////////////////////////////////////////////////////////

difference() {
	union() {
		FezCerberus(0,-50);

		LightSensor(50,-10);
		//N18Display(55,10);

		//rotate(-90) FezHydra(0,0);
		RelayX1(50, -20, -90);
	
		ENC28(50,-90,0);		
		UsbClientDP(0,-60, -90);

		BaseBoard(-10, -100,110, 115);
		Mount(-10,-110,110);		
		Top(-10,15,110);		
	}
	union() {
		#LightSensorCutOut(50,-10);
		#MoistureSensorCutOut(15,8);
		//N18DisplayCutOut(55,10);
		// screw holes in Mount
	}
}




AddModels();

