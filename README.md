# LaptopMonitor

Program used for checking status of battery charge, average CPU clock frequency and average CPU temperature. It uses Windows Services, JSON database in the form of a json file, plot drawing library LiveCharts and OpenHardwareMonitor's library providing necessary methods to fetch system's info.

This project's implementation is more complicated than it should've been due to it using Windows Services, but I've used them as an exercise. In the future I will probably make an reimplementation that is way more lightweight and doesn't require usage of installing a service on your computer.
