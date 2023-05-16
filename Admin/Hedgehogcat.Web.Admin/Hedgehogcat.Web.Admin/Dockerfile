

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
# 复制字体到 /usr/share/fonts 目录
COPY ./comic.ttf /usr/share/fonts/comic.ttf
# 容器对外暴露的端口，这里的 8090 与你监听的端口一致
EXPOSE 8090
# 指定工作目录
WORKDIR /app
# 将当前目录下所有文件复制到工作目录，注意 . . 中间是有空格的
COPY . .
# 设置环境变量，可设置多个，如urls...
ENV ASPNETCORE_ENVIRONMENT Development
ENV ASPNETCORE_URLS=http://+:8090
# 声明程序入口
ENTRYPOINT ["dotnet", "Hedgehogcat.Web.Admin.dll"]


