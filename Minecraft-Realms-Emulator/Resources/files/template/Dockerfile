FROM eclipse-temurin:21-jre-alpine

COPY . .

RUN apk upgrade
RUN apk add openssl

RUN wget -O rcon-cli.tar.gz https://github.com/itzg/rcon-cli/releases/download/1.6.7/rcon-cli_1.6.7_linux_amd64.tar.gz
RUN tar xzf rcon-cli.tar.gz

RUN rm rcon-cli.tar.gz
RUN mv rcon-cli /bin/rcon-cli

WORKDIR mc
RUN wget -O server.jar https://piston-data.mojang.com/v1/objects/450698d1863ab5180c25d7c804ef0fe6369dd1ba/server.jar

EXPOSE 25565
ENTRYPOINT ["/entrypoint"]