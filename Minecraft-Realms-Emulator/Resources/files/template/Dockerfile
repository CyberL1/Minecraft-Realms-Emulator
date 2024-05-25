FROM eclipse-temurin:21

WORKDIR /server
COPY . .

RUN mkdir mc
WORKDIR mc

RUN wget -O server.jar https://piston-data.mojang.com/v1/objects/145ff0858209bcfc164859ba735d4199aafa1eea/server.jar

EXPOSE 25565

RUN java -jar server.jar
RUN echo eula=true > eula.txt
CMD ["java", "-jar", "server.jar"]