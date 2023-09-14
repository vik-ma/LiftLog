import Button from "../../components/Button";
import { Text, View } from "../../components/Themed";

export default function TabOneScreen() {
  return (
    <View className="flex justify-center items-center h-full">
      <View className="flex">
        <Button label="asd" onPress={() => {}}></Button>
        {/* <Text className="text-red-500">TEST TEST asd</Text> */}
      </View>
    </View>
  );
}

// const styles = StyleSheet.create({
//   container: {
//     flex: 1,
//     alignItems: 'center',
//     justifyContent: 'center',
//   },
//   title: {
//     fontSize: 20,
//     fontWeight: 'bold',
//   },
//   separator: {
//     marginVertical: 30,
//     height: 1,
//     width: '80%',
//   },
// });
