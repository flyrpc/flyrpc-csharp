UNITY_DLL=/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll

# UNITY_DLL=C:\Program Files\Unity\Editor\Data\Managed\UnityEngine.dll

test: test.exe gen
	@mono test.exe

proto-gen/msg.cs: proto/msg.proto tool
	mkdir -p proto-gen
	mono tools/CodeGenerator.exe proto/msg.proto --output=proto-gen/msg.cs

gen: proto-gen/msg.cs

clean:
	rm -rf proto-gen

test.exe: flyrpc/*.cs
	#@mcs -r:$(UNITY_DLL) -out:test.exe flyrpc/*.cs
	@mcs -out:test.exe flyrpc/*.cs

tools/CodeGenerator.exe:
	@-mkdir tools
	cd tools &&\
		wget https://github.com/hultqvist/ProtoBuf/releases/download/2014-08-23/ProtoBuf-2014-08-23-bin.zip && \
		unzip ProtoBuf-2014-08-23-bin.zip

tool: tools/CodeGenerator.exe
