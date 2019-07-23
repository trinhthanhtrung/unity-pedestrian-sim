# Unity Pedestrian and Crowd Simulation

A collection of templates to add lively pedestrians into your environment. The templates make use of existing Unity NavMesh system for natural path finding and object avoiding.

Unity-PedSIM includes:

## 1. Pedestrian path follow
With this template, it's possible to
- Create a pedestrian path for each pedestrian
- Add multiple path nodes, each node can be set with different speed
- Wait for a number of seconds before start moving to next node
- Possible to be destroyed after reaching the end.

Demonstration video

[![](https://lh3.googleusercontent.com/24qy-iX1JkzKr4VB9bLEHoQ6oqe9fXar6CuNus_B5r8fKD8ouTtId6DS3QaZjRl06l-4INa04EiR7O4E-j_h3-sxhjlqM3gCycaX5zH2NJXHp-pFDcLsMCDx4DU8z5PbnPzeZ98J_eHcVWis_5qByX3LQhyHwB2bRHUs03wTes9J0lznAVpItF4dKUnG0FJKR9El33IjWkWEPeKYgiE9TVcoe2qFTdWGnECRPfnyTIM16zDric3OGw2VjjZT3A6WHQrOFYrI4wG3ANcxnIvR4NCIA2sqwIfx1xcDMpAz_FltP4jyBmjbvWm4kifHqtZaqo3v0q2xtUWDVvvBBZYgrNzKwxyHLmjte8zuiX1vyLBzy_NvQNc5g66y6IPrsEz9gb6jD4f620d87WgpYVBnzmnJZXNyAwNCDRLrgrFwgU6rhtn75l0l-LeAKg-Su-JyF2mXc04I4_R5p3jZ0Aky7IwaRIkNftaO0JV7xzB6JISSeEtrM_ErIBBMpjrHCqG807Vw2Ga1etn-ziSHq72fgupm01V1Q_IOLsAxlP_p__e5QmrtVRtwXGJ6oe_TEZLebkE8ot1tN6hfkNM0DMhmStmheLeatTCpmJAGTUg7R7srEMc1MI-yHTwf6r_vuMS0SS5mC6XQrEaU-ctt0bTLLfp2hTIfvNo=w854-h404-no)](http://www.youtube.com/watch?v=ZEOUTaBslIQ "")

## 2. Crowd Generation
With this template, it's possible to
- Use existing path from a Pedestrian Path and is able to randomly generate new pedestrians. 
- Randomise the paths: create a slight alternation to previous path so pedestrian will walk more naturally
- Boid-like movement: match surrounding speed, avoid other pedestrians.

Demonstration video

[![](https://lh3.googleusercontent.com/sHMiAfwilyt0vP90Jc5X1Jy8H8UfmdJR0oWg6RKj498UtVJmgUsvSALJfoL8MUr49bvqHEr65TYI4OyHdPff2pAekf0vouEXad7SRgMrjnJy9xAQ2bl2hPsLoXpvlFt8_Le7Wl_N0ve_jmDIUajCfvtnCZG7N-NSljYpQwDCy-5dMOghJbq3WN64FmH60GbRyX1KuRW3aVNKenK8zIHz6lOYoxWRU1iw_j1nc0deDZCvPcZv9SP8-sQ-9rFXvXtXA7VruDSVq_LuT2cQpyAGtxz8AwOIkjaOGw4lAwNFbXlEwLp_7fXfwFHRDu5t_6_0t_FJiO89PsGEfRmWqbGc_nNribldefhOqVWW8TBqwp7xtXeVQIJntyNqAbdBJTc_ZPuaZi2xf_elnShGU_OMxQoO2LxA4PbpJHZsqB6lcHUqUslp-EqLYwHHZEdZ2DI5XxEH-C1cuC1JbwsapY7zkCNKALtKSSLvR4cpd7Va0kpiqt0iJSVxuVeg2z4EJQ8WUuArpKYpF7hhv9ATqyHwJUc4-o15O5ykF_fL_JYKGd3z1Sv9NyE62d2igfHCvS_QOq3kfgB6Uk9Cs-CPTdzGEYqfDxQfkWDn9W0hxk0Cm7RWdls_WKIOSbSo-RvdcX4igSvfejzTFxuJZatMXFgw5_15Cyc885w=w854-h400-no)](http://www.youtube.com/watch?v=1i5-lEEf59o "")

## 3. Social Force Model pedestrian
This template is an implementation of the Social Force Model based on existing C++ codes from https://github.com/fawwazbmn/SocialForceModel. The model provides more natural walking behaviour for medium to large group of pedestrians.

All three templates can be found at *Example* scene.
