UNITY_DLL=/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll

# UNITY_DLL=C:\Program Files\Unity\Editor\Data\Managed\UnityEngine.dll

test: test.exe
	@mono test.exe

proto-gen/msg.cs: proto/msg.proto tools/CodeGenerator.exe
	mkdir -p proto-gen
	mono tools/CodeGenerator.exe --net2 proto/msg.proto --output=proto-gen/msg.cs

gen: proto-gen/msg.cs

clean:
	rm test.exe
	rm -rf proto-gen

test.exe: flyrpc/*.cs proto-gen/msg.cs
	#@cp proto-fix/*.cs proto-gen/
	#@mcs -r:$(UNITY_DLL) -out:test.exe flyrpc/*.cs
	@mcs -g -out:test.exe flyrpc/*.cs proto-gen/*.cs

tools/CodeGenerator.exe:
	@-mkdir tools
	cd tools &&\
		wget https://guileen.github.io/upload/protobuf-csharp-2015-04-08.zip && \
		unzip protobuf-csharp-2015-04-08.zip

tool: tools/CodeGenerator.exe
