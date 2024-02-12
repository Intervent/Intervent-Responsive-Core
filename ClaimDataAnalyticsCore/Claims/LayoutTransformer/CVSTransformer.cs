﻿namespace ClaimDataAnalytics.Claims.LayoutTransformer
{
    public sealed class CvsFixedLayoutTransformer : BaseFixedLayoutTransformer
    {
        protected override int[] Positions
        {
            get { return new int[] { 2, 3, 6, 16, 26, 41, 47, 67, 87, 287, 307, 342, 367, 368, 378, 433, 488, 518, 520, 535, 543, 544, 545, 546, 548, 568, 603, 628, 629, 639, 694, 749, 779, 781, 796, 804, 805, 807, 810, 813, 814, 815, 825, 840, 849, 864, 872, 873, 874, 875, 876, 877, 887, 888, 898, 900, 902, 906, 908, 923, 925, 940, 947, 982, 1037, 1092, 1122, 1124, 1139, 1142, 1152, 1154, 1155, 1156, 1166, 1168, 1183, 1185, 1200, 1203, 1213, 1215, 1250, 1275, 1285, 1287, 1302, 1305, 1340, 1365, 1366, 1367, 1369, 1370, 1379, 1381, 1400, 1408, 1416, 1422, 1430, 1435, 1443, 1445, 1447, 1448, 1456, 1464, 1479, 1480, 1481, 1489, 1519, 1528, 1536, 1546, 1548, 1551, 1559, 1560, 1562, 1563, 1565, 1567, 1568, 1578, 1588, 1591, 1593, 1594, 1596, 1598, 1613, 1615, 1630, 1632, 1647, 1649, 1664, 1666, 1681, 1683, 1685, 1687, 1689, 1691, 1693, 1695, 1697, 1699, 1701, 1703, 1705, 1707, 1709, 1711, 1713, 1715, 1717, 1719, 1721, 1723, 1725, 1727, 1729, 1731, 1733, 1735, 1737, 1739, 1741, 1743, 1745, 1747, 1749, 1751, 1753, 1755, 1774, 1775, 1778, 1781, 1784, 1787, 1790, 1820, 1828, 1829, 1859, 1889, 1904, 1908, 1914, 1916, 1917, 1918, 1919, 1920, 1921, 1923, 1925, 1927, 1928, 1934, 1940, 1954, 1955, 1958, 1960, 1963, 1965, 1971, 1972, 1973, 1981, 1996, 1997, 2005, 2013, 2021, 2029, 2037, 2045, 2053, 2061, 2069, 2077, 2078, 2080, 2081, 2089, 2098, 2106, 2107, 2117, 2125, 2133, 2142, 2150, 2158, 2166, 2174, 2181, 2183, 2191, 2199, 2201, 2209, 2211, 2219, 2221, 2229, 2237, 2238, 2248, 2249, 2259, 2267, 2269, 2277, 2285, 2293, 2301, 2309, 2317, 2325, 2333, 2335, 2337, 2339, 2341, 2343, 2344, 2352, 2353, 2354, 2362, 2370, 2378, 2379, 2383, 2394, 2396, 2407, 2417, 2418, 2420, 2423, 2424, 2454, 2462, 2463, 2464, 2474, 2482, 2490, 2498, 2506, 2514, 2524, 2532, 2540, 2548, 2556, 2564, 2594, 2614, 2623, 2627, 2632, 2634, 2649, 2659, 2660, 2661, 2664, 2684, 2685, 2686, 2687, 2688, 2689, 2697, 2718, 2726, 2727, 2728, 2739, 4000 }; }
        }

        protected override bool IsInsert { get { return true; } }
    }
}
