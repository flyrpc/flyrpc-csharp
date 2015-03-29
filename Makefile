UNITY_DLL=/Applications/Unity/Unity.app/Contents/Frameworks/Managed/UnityEngine.dll
# UNITY_DLL=C:\Program Files\Unity\Editor\Data\Managed\UnityEngine.dll
proto-gen/msg.cs: proto/msg.proto
	mkdir -p proto-gen
	mono tools/CodeGenerator.exe proto/msg.proto --output=proto-gen/msg.cs

gen: proto-gen/msg.cs

clean:
	rm -rf proto-gen

test.exe: flyrpc/*.cs
	#@mcs -r:$(UNITY_DLL) -out:test.exe flyrpc/*.cs
	@mcs -out:test.exe flyrpc/*.cs

test: test.exe
	@mono test.exe
