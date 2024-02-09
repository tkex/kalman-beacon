import numpy as np
import warnings

def find_intersection_point(p_1, p_2, dir_1, dir_2):
    with warnings.catch_warnings(record=True) as w:
        # Calculate the scalar values for the two lines
        scalar = np.cross(p_2 - p_1, dir_2) / np.cross(dir_1, dir_2)
        # Calculate the unknown point
        intersection = p_1 + scalar * dir_1
        
        # if we get a division error, there are infinitely many intersections 
        #   i.e. dir_1 and dir_2 run through both p_1 and p_2
        if len(w) > 0:
                   return None
       
    return intersection 

point_1 = np.array([-30, 0])
point_2 = np.array([0, 30])
point_3 = np.array([30, 0])
direction_1 = np.array([-1, 0])  
direction_2 = np.array([0, 1]) 
direction_3 = np.array([1, 0]) 

intersection_1_2 = find_intersection_point(point_1, point_2, direction_1, direction_2)
intersection_1_3 = find_intersection_point(point_1, point_3, direction_1, direction_3)
intersection_2_3 = find_intersection_point(point_2, point_3, direction_2, direction_3)

print("Intersection 1-2:", intersection_1_2)
print("Intersection 1-3:", intersection_1_3)
print("Intersection 2-3:", intersection_2_3)


# jetzt aus diesen 3 Punkten den Punkten ein Dreieck bilden und 

x1 = intersection_1_2[0] if intersection_1_2 is not None else 0
x2 = intersection_1_3[0] if intersection_1_3 is not None else 0
x3 = intersection_2_3[0] if intersection_2_3 is not None else 0

y1 = intersection_1_2[1] if intersection_1_2 is not None else 0
y2 = intersection_1_3[1] if intersection_1_3 is not None else 0
y3 = intersection_2_3[1] if intersection_2_3 is not None else 0


# find midpoint of intersections
midpoint_x = (x1 + x2 + x3)/3;
midpoint_y = (y1 + y2 + y3)/3;

midpoint = np.array([midpoint_x, midpoint_y])

print("Midpoint:", midpoint)
