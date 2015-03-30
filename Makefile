UNITY_DLL=/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll

# UNITY_DLL=C:\Program Files\Unity\Editor\Data\Managed\UnityEngine.dll

test: test.exe
	@mono test.exe

proto-gen/msg.cs: proto/msg.proto tools/CodeGenerator.exe
	mkdir -p proto-gen
	mono tools/CodeGenerator.exe proto/msg.proto --output=proto-gen/msg.cs

gen: proto-gen/msg.cs

clean:
	rm test.exe
	rm -rf proto-gen

test.exe: flyrpc/*.cs proto-gen/msg.cs
	@cp proto-fix/*.cs proto-gen/
	#@mcs -r:$(UNITY_DLL) -out:test.exe flyrpc/*.cs
	@mcs -g -out:test.exe flyrpc/*.cs proto-gen/*.cs

tools/CodeGenerator.exe:
	@-mkdir tools
	cd tools &&\
		wget https://github.com/hultqvist/ProtoBuf/releases/download/2014-08-23/ProtoBuf-2014-08-23-bin.zip && \
		unzip ProtoBuf-2014-08-23-bin.zip

tool: tools/CodeGenerator.exe
