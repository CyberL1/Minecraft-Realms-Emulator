FROM eclipse-temurin:21-jre-alpine

WORKDIR /server
COPY . .

RUN mkdir mc
WORKDIR mc

RUN apk upgrade
RUN apk add openssl

RUN wget -O rcon-cli.tar.gz https://github.com/itzg/rcon-cli/releases/download/1.6.7/rcon-cli_1.6.7_linux_amd64.tar.gz
RUN tar xzf rcon-cli.tar.gz

RUN rm rcon-cli.tar.gz
RUN mv rcon-cli /bin/rcon-cli

RUN wget -O server.jar https://piston-data.mojang.com/v1/objects/450698d1863ab5180c25d7c804ef0fe6369dd1ba/server.jar

EXPOSE 25565

RUN java -jar server.jar

RUN echo "$(openssl rand -hex 20)" > .rcon-pwd

RUN echo eula=true > eula.txt
RUN echo "enable-rcon=true" >> server.properties
RUN echo "rcon.password=$(cat .rcon-pwd)" >> server.properties

RUN echo "password: $(cat .rcon-pwd)" >> /root/.rcon-cli.yaml

RUN rm .rcon-pwd
CMD ["java", "-jar", "server.jar"]