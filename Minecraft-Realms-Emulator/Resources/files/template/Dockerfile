FROM eclipse-temurin:21-jre-alpine
COPY . .

RUN apk upgrade && \
    apk add openssl curl jq && \
    wget -O rcon-cli.tar.gz https://github.com/itzg/rcon-cli/releases/download/1.6.7/rcon-cli_1.6.7_linux_amd64.tar.gz && \
    tar xzf rcon-cli.tar.gz && \
    rm rcon-cli.tar.gz && \
    mv rcon-cli /bin/rcon-cli

WORKDIR /mc
ENTRYPOINT ["/entrypoint"]