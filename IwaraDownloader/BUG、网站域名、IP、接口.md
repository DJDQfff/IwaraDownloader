﻿* 域名：www.iwara.com(全年龄)

    ecchi.iwara.com(R18)

* 地址：/videos（视频）
  	 和其他（没记）
* IP（可用）：	
  	https://66.206.15.50/videos
* BUG：
  	（我只能手动修好电脑host缓存文件，软件才能正常访问域名，直接访问IP无法使用）
* 新功能：
  	自己访问DNS服务器、寻找最优ip的，而不必用户手动寻找合适的ip
* 冲突：
  	使用getasync（）方法时，访问的时www.iwara.tv/videos
  	如果设置httpclient的hostname为ecchi.iwara.tv，则返回的却是ecchi.iwara.tv/videos
  	不设置或者设置为www.iwara.tv，则返回是www.iwara.tv
* 使用数据库