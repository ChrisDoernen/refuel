import { graphql, useFragment } from "react-relay";
import type { User_item$key } from "./__generated__/User_item.graphql";

export default function FilmListItem(props: { user: User_item$key; }) {
  const user = useFragment<User_item$key>(
    graphql`
      fragment User_item on User {
        firstName
        lastName
      }
    `,
    props.user
  );

  return (
    <li>
      <b>{user.firstName}</b>: directed by <i>{user.lastName}</i>
    </li>
  );
}